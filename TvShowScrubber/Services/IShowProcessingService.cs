using TvShowScrubber.Models;

namespace TvShowScrubber.Services
{
    public interface IShowProcessingService
    {
        Task<List<Show>> GetShowsWithoutCastAsync(int lastShowId);

        Task<List<Cast>> GetCastForShowRangeAsync(List<Show> shows);

        Task<ShowWithCastEmbedded> GetShowWithEmbeddedCastAsync(int showId);

        Task<Dictionary<int, int>> GetShowsListAsync();
    }
}
