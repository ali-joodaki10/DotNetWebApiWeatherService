namespace WeatherApi.ExternalServices;

public sealed class WeatherApiClient : IWeatherApiClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<WeatherApiClient> _logger;

    public WeatherApiClient(HttpClient httpClient, ILogger<WeatherApiClient> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<string?> GetWeatherApiClient(CancellationToken cancellationToken)
    {
        try
        {
            var response = await _httpClient.GetAsync("forecast?latitude=52.52&longitude=13.41&hourly=temperature_2m", cancellationToken);
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync(cancellationToken);
            return content;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching weather data from external API.");
            return null;
        }
    }
}