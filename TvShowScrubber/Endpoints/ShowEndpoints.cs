using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TvShowScrubber.Constants;
using TvShowScrubber.Contexts;
using TvShowScrubber.Models;
using TvShowScrubber.Services;

namespace TvShowScrubber.Endpoints;

public class ShowEndpoints
{
    public static void MapShowEndpoints(WebApplication app, IConfiguration configuration)
    {
        app.MapGet("/shows", async (
            [FromQuery(Name = "page")] int page,
            [FromQuery(Name = "pageSize")] int? pageSize,
            ShowsDb db,
            IShowProcessingService showProcessingService)
          =>
        {
            var effectivePageSize = pageSize ?? 50;
            List<ShowResponse> showsResponse = new();
            if (bool.TryParse(configuration[SettingConstants.PreloadCast], out var preLoadCast) && preLoadCast)
            {
                var shows = await db.ShowsWithCastEmbedded
                .Skip((page - 1) * effectivePageSize)
                .Take(effectivePageSize)
                .ToListAsync();

                showsResponse = CreateShowResponseWithEmbeddedCast(shows, db);
            }
            else
            {
                var shows = await db.Shows
                .Skip((page - 1) * effectivePageSize)
                .Take(effectivePageSize)
                .ToListAsync();

                var castForShow = await showProcessingService.GetCastForShowRangeAsync(shows);

                showsResponse = CreateShowResponse(shows, castForShow);
            }

            return showsResponse;
        });
    }

    private static List<ShowResponse> CreateShowResponse(List<Show> shows, List<Cast> castForShow)
    {
        List<ShowResponse> showResponse = new();
        foreach (Show show in shows)
        {
            var response = new ShowResponse
            {
                Id = show.Id,
                Name = show.Name,
                Cast = castForShow.Where(x => x.ShowId == show.Id).OrderBy(x => x.Birthday).ToList(),
            };
            showResponse.Add(response);
        }

        return showResponse;
    }

    private static List<ShowResponse> CreateShowResponseWithEmbeddedCast(List<ShowWithCastEmbedded> shows, ShowsDb db)
    {
        List<ShowResponse>? showReponses = new();
        foreach (var show in shows)
        {
            var response = new ShowResponse
            {
                Id = show.Id,
                Name = show.Name
            };

            var castList = new List<Cast>();
            if (show.Cast != null)
            {
                foreach (var cast in show.Cast.CastOverviews.OrderBy(c => c.Person.Birthday))
                {
                    castList.Add(new Cast
                    {
                        Id = cast.Person.Id,
                        Name = cast.Person.Name,
                        Birthday = cast.Person.Birthday
                    });
                }
            }
            else
            {
               response.Cast = db.Casts.Where(cast => cast.ShowId == show.Id).OrderBy(c => c.Birthday).ToList();
            }
            showReponses.Add(response);
        }
        return showReponses;
    }
}

