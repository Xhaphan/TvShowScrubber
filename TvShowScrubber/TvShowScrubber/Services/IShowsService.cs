using TvShowScrubber.Models;

namespace TvShowScrubber.Services;

public interface IShowsService
{
    Task<List<Show>> GetShowsAsync(int pageNumber, int pageSize);
}