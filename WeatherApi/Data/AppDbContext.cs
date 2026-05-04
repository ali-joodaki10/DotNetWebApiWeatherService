using Microsoft.EntityFrameworkCore;
using WeatherApi.Entities;

namespace WeatherApi.Data;

public sealed class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<WeatherRecord> WeatherRecords { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<WeatherRecord>()
            .HasIndex(w => w.CreatedAt)
            .HasDatabaseName("IX_WeatherRecords_CreatedAt");
    }
}
