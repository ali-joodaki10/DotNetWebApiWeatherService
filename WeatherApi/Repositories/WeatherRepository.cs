using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using WeatherApi.Data;
using WeatherApi.Entities;

namespace WeatherApi.Repositories;

public sealed class WeatherRepository : IWeatherRepository
{
    private readonly AppDbContext _context;
    private readonly string _connectionString;
    private const int MaxRows = 10;

    public WeatherRepository(AppDbContext context, IConfiguration configuration)
    {
        _context = context;
        _connectionString = configuration.GetConnectionString("DefaultConnection")!;
    }

    public async Task<string> Create(WeatherRecord weatherRecord, CancellationToken cancellationToken)
    {

        await _context.WeatherRecords.AddAsync(weatherRecord, cancellationToken);

        await _context.SaveChangesAsync(cancellationToken);

        return weatherRecord.RawJson;

    }

    public async Task<string?> GetLatest(CancellationToken cancellationToken)
    {
        return await _context.WeatherRecords.OrderByDescending(x => x.CreatedAt)
                                            .Select(x => x.RawJson)
                                            .AsNoTracking()
                                            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task Cleanup()
    {
        using var conn = new SqlConnection(_connectionString);
        await conn.OpenAsync();

        int total = await conn.ExecuteScalarAsync<int>("SELECT COUNT(*) FROM WeatherRecords");

        if (total < MaxRows)
            return;

        int toDelete = total - MaxRows;

        string sql = @"
            ;WITH OldRows AS (
                SELECT TOP (@CountToDelete) Id
                FROM WeatherRecords
                ORDER BY CreatedAt ASC
            )
            DELETE FROM WeatherRecords
            WHERE Id IN (SELECT Id FROM OldRows);
        ";

        await conn.ExecuteAsync(sql, new { CountToDelete = toDelete });
    }
}
