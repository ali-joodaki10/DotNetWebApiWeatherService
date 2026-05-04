
namespace WeatherApi.Services;

public interface IWeatherService
{
    Task<string?> GetWeather(CancellationToken token);
}
