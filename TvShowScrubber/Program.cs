using Microsoft.EntityFrameworkCore;
using Microsoft.Net.Http.Headers;
using Polly;
using Polly.Extensions.Http;
using System.Net;
using TvShowScrubber.Constants;
using TvShowScrubber.Contexts;
using TvShowScrubber.Endpoints;
using TvShowScrubber.Services;

var builder = WebApplication.CreateBuilder(args);
var baseAddress = builder.Configuration[SettingConstants.HttpClientBaseAddress] ?? "https://api.tvmaze.com";

if (!int.TryParse(builder.Configuration[SettingConstants.HttpClientTimeOutInSeconds], out int timeOutInSeconds))
    timeOutInSeconds = 600;

builder.Services.AddEndpointsApiExplorer();

//builder.Services.AddDbContext<ShowsDb>(options =>
//{
//    options.UseInMemoryDatabase("items");
//    options.EnableSensitiveDataLogging();
//});
var connectionString = builder.Configuration.GetConnectionString("Shows") ?? "Data Source=Shows.db";
builder.Services.AddSqlite<ShowsDb>(connectionString);

builder.Services.AddSwaggerGen();


builder.Services.AddHttpClient<IShowProcessingService, ShowProcessingService>(client =>
    {
        client.BaseAddress = new Uri(baseAddress);
        client.DefaultRequestHeaders.Add(HeaderNames.UserAgent, nameof(ShowsBackgroundService));
        client.Timeout = TimeSpan.FromSeconds(timeOutInSeconds);
    })
    .SetHandlerLifetime(Timeout.InfiniteTimeSpan)
    .AddPolicyHandler(GetRetryPolicy());

builder.Services.AddHostedService<ShowsBackgroundService>();

var app = builder.Build();

ShowEndpoints.MapShowEndpoints(app, app.Configuration);

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.Run();


static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
{
    return HttpPolicyExtensions
        .HandleTransientHttpError()
        .OrResult(msg => msg.StatusCode == HttpStatusCode.TooManyRequests)
        .WaitAndRetryAsync(10, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));
}