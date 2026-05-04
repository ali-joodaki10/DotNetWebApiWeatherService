using WeatherApi.Entities;

namespace WeatherApi.Repositories;

public interface IWeatherRepository
{
    Task<string> Create(WeatherRecord weatherRecord, CancellationToken cancellationToken);
    Task<string?> GetLatest(CancellationToken cancellationToken);
    Task Cleanup();
}
