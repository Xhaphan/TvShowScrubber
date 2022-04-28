using TvShowScrubber.Endpoints;
using Microsoft.EntityFrameworkCore;
using TvShowScrubber.Services;
using TvShowScrubber.Models;
using Polly;
using Polly.Extensions.Http;
using TvShowScrubber.Contexts;
using Microsoft.Net.Http.Headers;
using System.Net;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddDbContext<ShowsDb>(options =>
{
    options.UseInMemoryDatabase("items");
    options.EnableSensitiveDataLogging();
}, ServiceLifetime.Singleton);
//var connectionString = builder.Configuration.GetConnectionString("Shows") ?? "Data Source=Shows.db";
//builder.Services.AddSqlite<ShowsDb>(connectionString);

builder.Services.AddSwaggerGen();

builder.Services.AddHttpClient(nameof(ShowsService), client =>
    {
        client.BaseAddress = new Uri("https://api.tvmaze.com");
        client.DefaultRequestHeaders.Add(HeaderNames.UserAgent, nameof(ShowsService));
        client.Timeout = TimeSpan.FromSeconds(600);
    })
    .SetHandlerLifetime(TimeSpan.FromMinutes(10))
    .AddPolicyHandler(GetRetryPolicy());

builder.Services.AddHostedService<ShowsService>();

var app = builder.Build();

ShowEndpoints.MapShowEndpoints(app);

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
        .OrResult(msg => msg.StatusCode == HttpStatusCode.RequestTimeout || msg.StatusCode == HttpStatusCode.GatewayTimeout)
        .WaitAndRetryAsync(10, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2,
                                                                    retryAttempt)));
}