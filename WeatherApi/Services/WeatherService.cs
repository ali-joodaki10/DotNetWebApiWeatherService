using WeatherApi.Entities;
using WeatherApi.ExternalServices;
using WeatherApi.Repositories;

namespace WeatherApi.Services;

public class WeatherService : IWeatherService
{
    private readonly IWeatherApiClient _weatherApiClient;
    private readonly IWeatherRepository _weatherRepository;
    private readonly ILogger<WeatherService> _logger;

    public WeatherService(
        IWeatherApiClient weatherApiClient,
        IWeatherRepository weatherRepository,
        ILogger<WeatherService> logger)
    {
        _weatherApiClient = weatherApiClient;
        _weatherRepository = weatherRepository;
        _logger = logger;
    }

    public async Task<string?> GetWeather(CancellationToken token)
    {
        try
        {
            var jsonData = await _weatherApiClient.GetWeatherApiClient(token);

            if (string.IsNullOrWhiteSpace(jsonData))
                return await GetFallbackWeather(token);

            await SaveWeather(jsonData, token);

            return jsonData;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while getting weather from API. Falling back to latest stored data.");
            return await GetFallbackWeather(token);
        }
    }

    private async Task SaveWeather(string jsonData, CancellationToken token)
    {
        try
        {
            var record = new WeatherRecord
            {
                CreatedAt = DateTime.UtcNow,
                RawJson = jsonData
            };

            await _weatherRepository.Create(record, token);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Weather data was fetched successfully, but saving to database failed.");
        }
    }

    private async Task<string?> GetFallbackWeather(CancellationToken token)
    {
        var fallback = await _weatherRepository.GetLatest(token);

        if (string.IsNullOrWhiteSpace(fallback))
        {
            _logger.LogWarning("No weather data found in database for fallback.");
            return null;
        }

        return fallback;
    }
}