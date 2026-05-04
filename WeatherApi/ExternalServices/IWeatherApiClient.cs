namespace WeatherApi.ExternalServices;

public interface IWeatherApiClient
{
    Task<string?> GetWeatherApiClient(CancellationToken cancellationToken);
}
