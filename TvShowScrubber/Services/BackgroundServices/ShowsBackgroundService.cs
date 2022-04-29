using Microsoft.EntityFrameworkCore;
using TvShowScrubber.Constants;
using TvShowScrubber.Contexts;
using TvShowScrubber.Models;

namespace TvShowScrubber.Services
{
    public class ShowsBackgroundService : BackgroundService
    {
        private readonly ShowsDb _dbShows;
        private readonly IConfiguration _configuration;
        private readonly IShowProcessingService _showProcessingService;

        public ShowsBackgroundService(IServiceProvider serviceProvide, IConfiguration configuration)
        {
            _dbShows = serviceProvide.CreateScope().ServiceProvider.GetRequiredService<ShowsDb>();
            _showProcessingService = serviceProvide.CreateScope().ServiceProvider.GetRequiredService<IShowProcessingService>();
            _configuration = configuration;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                if (bool.TryParse(_configuration[SettingConstants.PreloadCast], out var preLoadCast) && !preLoadCast)
                {
                    int lastShowId = _dbShows.Shows.Any() ? _dbShows.Shows.OrderBy(show => show.Id).Last().Id : 0;
                    List<Show> processedShows = await _showProcessingService.GetShowsWithoutCastAsync(lastShowId);

                    await _dbShows.Shows.AddRangeAsync(processedShows);
                }
                else
                {
                    await ProcessShowsWithEmbeddedCast();
                }
            }
            catch (TaskCanceledException taskEx)
            {
                await _dbShows.SaveChangesAsync();
                await ExecuteAsync(stoppingToken);
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                await _dbShows.SaveChangesAsync();
            }
        }

        private async Task ProcessShowsWithEmbeddedCast()
        {
            Dictionary<int, int> showsToFetch = await _showProcessingService.GetShowsListAsync();

            if (showsToFetch == null)
                throw new Exception("Unable to retrieve list of shows.");

            List<Task<ShowWithCastEmbedded>> tasks = new();

            var existingShows = await _dbShows.ShowsWithCastEmbedded.ToListAsync();
            var newShows = showsToFetch.Where(show => !existingShows.Any(s => s.Id == show.Key));

            if (!int.TryParse(_configuration[SettingConstants.ConcurrentRequestsThresholdWithCast], out int threshold))
                threshold = 500;

            foreach ((var key, var value) in newShows)
            {
                tasks.Add(_showProcessingService.GetShowWithEmbeddedCastAsync(key));

                var runningTasks = tasks.Where(task => !task.IsCompleted).Count();
                while (runningTasks > threshold)
                {
                    Thread.Sleep(1000);
                    runningTasks = tasks.Where(t => !t.IsCompleted).Count();
                }
            }

            while (tasks.Any())
            {
                var show = await Task.WhenAny(tasks);
                tasks.Remove(show);
                await _dbShows.ShowsWithCastEmbedded.AddAsync(await show);
            }
            //var showsWithEmbeddedCast = await Task.WhenAll(tasks);
            //await _dbShows.ShowsWithCastEmbedded.AddRangeAsync(showsWithEmbeddedCast);
        }
    }
}
