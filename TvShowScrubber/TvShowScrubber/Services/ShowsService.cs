using System.Diagnostics;
using System.Text.Json;
using TvShowScrubber.Contexts;
using TvShowScrubber.Models;

namespace TvShowScrubber.Services
{
    public class ShowsService : BackgroundService
    {
        private readonly HttpClient _httpClient;
        private readonly ShowsDb _dbShows;

        public ShowsService(IHttpClientFactory httpClientFactory, ShowsDb dbShows)
        {
            _httpClient = httpClientFactory.CreateClient(nameof(ShowsService));
            _dbShows = dbShows;
        }

        public async Task<List<Show>> GetShowsAsync(int pageNumber)
        {
            var result = await _httpClient.GetAsync($"/shows?page={pageNumber}");

            if (result?.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return null;
            }

            var content = await result.Content.ReadAsStringAsync();
            var shows = JsonSerializer.Deserialize<List<Show>>(content);

            Debug.WriteLine($"Page number {pageNumber}");
            return shows;
        }

        public async Task<List<Cast>> GetCastForShowAsync(int showId)
        {
            var result = await _httpClient.GetAsync($"/shows/{showId}/cast");
            if (result?.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return null;
            }

            var content = await result.Content.ReadAsStringAsync();
            var castOverviews = JsonSerializer.Deserialize<List<CastOverview>>(content);

            var cast = new List<Cast>();
            foreach (var castPerson in castOverviews)
            {
                var castToAdd = castPerson.Person;
                castToAdd.ShowId = showId;
                cast.Add(castToAdd);
            }

            Debug.WriteLine($"Adding cast for {showId}");
            return cast;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var stopwatch = Stopwatch.StartNew();
            int pageCounter = 0;
            var castTasks = new List<Task>();

            while (!stoppingToken.IsCancellationRequested && pageCounter < 1)
            {
                var shows = await GetShowsAsync(pageCounter);
                if (shows == null)
                    break;

                await _dbShows.Shows.AddRangeAsync(shows);

                var runningTasksCount = castTasks.Where(t => !t.IsCompleted).Count();
                if (runningTasksCount > 10)
                {
                    while (runningTasksCount > 10)
                    {
                        Debug.WriteLine($"zzzzzzzzzzzzzz {runningTasksCount}");
                        Thread.Sleep(1000);
                        runningTasksCount = castTasks.Where(t => !t.IsCompleted).Count();
                    }
                }
                castTasks.Add(AddCastForShowRange(shows));
                pageCounter++;
            }

            await Task.WhenAll(castTasks);

            await _dbShows.SaveChangesAsync();

            stopwatch.Stop();
            Debug.WriteLine($"********{stopwatch.Elapsed}");
        }

        private async Task AddCastForShowRange(List<Show> shows)
        {
            var castList = new List<Cast>();
            foreach (var show in shows)
            {
                var cast = await GetCastForShowAsync(show.Id);
                if (cast != null)
                    castList.AddRange(cast);
            }
            await _dbShows.Casts.AddRangeAsync(castList);
        }
    }
}
