# TvShowScrubber

Scrubs an API for TV Show and Cast data, and stores it in a local SQLite Database.
It is wrtten in .NET 6 and with the new Minimal API style.

## Method 1

*This is the default, and recommended method. Used by configuring "PreloadCast" as "false" in appsettings.json*  
This method only scrubs the show data on startup. It then retrieves the cast data for the shows when required. This allows a relatively quick startup, as well as minifying the risk of getting rate limited by the TV Api.

This method will also only retrieve show data that has not already been retrieved, which means subsequent startup times will be a lot faster.

## Method 2 - *beta*

This method scrubs show data combined with cast data. It backs off and has a threshold for how many requests can run concurrently.

**Note** This method can take up to an hour to run initially, after which it will only run for show data not already in the database.

## Rate Limiting

One of the major challenges is the rate limiting encountered by the TV API. This is overcome by various techniques with retry policies as well as thresholds.

## Where to from here

Ideally you would want the background service to be a standalone service running nightly. This will ensure our database is always up to date.

It would also check for deleted and updated shows to keep our database up to date.

Method 2 would also be the ideal situation in this regard, but would need a bit more work to be resilient.

Improved error handling would also be implemented, as well as unit tests.

It also seems like a NoSQL database might have been a better decision.
