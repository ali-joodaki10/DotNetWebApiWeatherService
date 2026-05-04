using Microsoft.AspNetCore.Http.Timeouts;
using Microsoft.EntityFrameworkCore;
using Polly;
using Polly.Extensions.Http;
using WeatherApi.BackgroundJobs;
using WeatherApi.Data;
using WeatherApi.ExternalServices;
using WeatherApi.Repositories;
using WeatherApi.Services;
using WeatherCollector.Api.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

//builder.Services.AddRequestTimeouts(options =>
//{
//    options.DefaultPolicy = new RequestTimeoutPolicy
//    {
//        Timeout = TimeSpan.FromSeconds(5)
//    };
//});

// DB
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Repositories
builder.Services.AddScoped<IWeatherRepository, WeatherRepository>();

// Services
builder.Services.AddScoped<IWeatherService, WeatherService>();

// Retention Worker
builder.Services.AddHostedService<RetentionWorker>();

builder.Services
    .AddHttpClient<IWeatherApiClient, WeatherApiClient>(client =>
    {
        client.BaseAddress = new Uri("https://api.open-meteo.com/v1/");
        client.Timeout = TimeSpan.FromSeconds(3);
    })
    .AddPolicyHandler(GetRetryPolicy())
    .AddPolicyHandler(GetCircuitBreakerPolicy())
    .AddPolicyHandler(Policy.TimeoutAsync<HttpResponseMessage>(2));

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.UseMiddleware<RequestTimeoutMiddleware>();

app.Run();

static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
{
    return HttpPolicyExtensions
        .HandleTransientHttpError()
        .WaitAndRetryAsync(2, retryNumber => TimeSpan.FromMilliseconds(200 * retryNumber));
}

static IAsyncPolicy<HttpResponseMessage> GetCircuitBreakerPolicy()
{
    return HttpPolicyExtensions
        .HandleTransientHttpError()
        .CircuitBreakerAsync(5, TimeSpan.FromSeconds(30));
}