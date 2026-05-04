using WeatherApi.Repositories;

namespace WeatherApi.BackgroundJobs;

public sealed class RetentionWorker : BackgroundService
{
    private readonly ILogger<RetentionWorker> _logger;
    private readonly IServiceScopeFactory _scopeFactory;

    public RetentionWorker(IServiceScopeFactory scopeFactory, ILogger<RetentionWorker> logger)
    {
        _scopeFactory = scopeFactory; _logger = logger;
    }


    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var timer = new PeriodicTimer(TimeSpan.FromDays(1));

        while (await timer.WaitForNextTickAsync(stoppingToken))
        //while (true)
        {
            try
            {
                _logger.LogInformation("Starting database cleanup operation in the background");

                using var scope = _scopeFactory.CreateScope();

                var repo = scope.ServiceProvider.GetRequiredService<IWeatherRepository>();

                await repo.Cleanup();

                _logger.LogInformation("The old record was successfully deleted.");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error clearing database: {ex.Message}");
            }
        }
    }
}
