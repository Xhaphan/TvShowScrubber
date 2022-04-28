using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TvShowScrubber.Contexts;
using TvShowScrubber.Models;
using TvShowScrubber.Services;

namespace TvShowScrubber.Endpoints;

public class ShowEndpoints
{
    public static void MapShowEndpoints(WebApplication app)
    {
        app.MapGet("/shows", async ([FromQuery(Name = "page")] int page,
            [FromQuery(Name = "pageSize")] int pageSize,
            ShowsDb db)
          =>
        {
            var shows = await db.Shows
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

            var showsResponse = new List<ShowResponse>();
            foreach (var show in shows)
            {
                var cast = await db.Casts.Where(x => x.ShowId == show.ShowId).OrderBy(c => c.Birthday).ToListAsync();
                showsResponse.Add(new ShowResponse
                {
                    ShowId = show.ShowId,
                    Name = show.Name,
                    Cast = cast
                });
            }

            return showsResponse;
        });
    }
}

