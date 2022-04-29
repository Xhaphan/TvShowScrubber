using System.Text.Json;
using TvShowScrubber.Constants;
using TvShowScrubber.Models;

namespace TvShowScrubber.Services
{
    public class ShowProcessingService : IShowProcessingService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        bool _shouldContinue = true;

        public ShowProcessingService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
        }

        public async Task<List<Show>> GetShowsWithoutCastAsync(int lastShowId)
        {
            decimal lastPageCalc = (decimal)lastShowId / 250m;
            int pageCounter = (int)Math.Floor(lastPageCalc);
            List<Task<List<Show>>> tasks = new();

            while (_shouldContinue)
            {
                tasks.Add(GetShowsAsync(pageCounter));
                pageCounter++;
            }

            var shows = await Task.WhenAll(tasks);

            return shows.SelectMany(s => s).Where(i => i.Id > lastShowId).ToList();
        }

        public async Task<List<Cast>> GetCastForShowRangeAsync(List<Show> shows)
        {
            List<Task<List<Cast>>> tasks = new();
            if (!int.TryParse(_configuration[SettingConstants.ConcurrentThresholdShowsOnly], out int threshold))
                threshold = 50;

            foreach (var show in shows)
            {
                tasks.Add(GetCastForShowAsync(show.Id));

                var runningTaskCount = tasks.Where(t => !t.IsCompleted).Count();
                while (runningTaskCount > threshold)
                {
                    Thread.Sleep(1000);
                    runningTaskCount = tasks.Where(t => !t.IsCompleted).Count();
                }
            }

            var cast = await Task.WhenAll(tasks);
            return cast.SelectMany(c => c).ToList();
        }

        public async Task<ShowWithCastEmbedded> GetShowWithEmbeddedCastAsync(int showId)
        {
            var result = await _httpClient.GetAsync($"/shows/{showId}?embed=cast");

            result.EnsureSuccessStatusCode();

            var content = await result.Content.ReadAsStringAsync();
            var show = JsonSerializer.Deserialize<ShowWithCastEmbedded>(content);

            if (show == null)
                throw new Exception($"Unable to retrieve show {showId}");

            show.Cast.CastOverviews.ForEach(c => c.Person.ShowId = showId);
            return show;
        }

        public async Task<Dictionary<int, int>> GetShowsListAsync()
        {
            Dictionary<int, int> showList = new();

            var result = await _httpClient.GetAsync("/updates/shows");
            result.EnsureSuccessStatusCode();

            var content = await result.Content.ReadAsStringAsync();
            var showsToFetch = JsonSerializer.Deserialize<Dictionary<int, int>>(content);

            if (showsToFetch != null)
                showList = showsToFetch;

            return showList;
        }

        private async Task<List<Show>> GetShowsAsync(int pageNumber)
        {
            var result = await _httpClient.GetAsync($"/shows?page={pageNumber}");
            var shows = new List<Show>();

            if (result?.StatusCode == System.Net.HttpStatusCode.NotFound || result == null)
            {
                _shouldContinue = false;
                return shows;
            }

            var content = await result.Content.ReadAsStringAsync();
            var responseShows = JsonSerializer.Deserialize<List<Show>>(content);

            if (responseShows != null)
                shows.AddRange(responseShows);

            return shows;
        }

        private async Task<List<Cast>> GetCastForShowAsync(int showId)
        {
            var cast = new List<Cast>();
            var result = await _httpClient.GetAsync($"/shows/{showId}/cast");
            if (result?.StatusCode == System.Net.HttpStatusCode.NotFound || result == null)
            {
                return cast;
            }

            var content = await result.Content.ReadAsStringAsync();
            var castOverviews = JsonSerializer.Deserialize<List<CastOverview>>(content);

            if (castOverviews != null)
            {
                foreach (var castPerson in castOverviews)
                {
                    var castToAdd = castPerson.Person;
                    castToAdd.ShowId = showId;
                    cast.Add(castToAdd);
                }
            }

            return cast;
        }
    }
}
