namespace WeatherApi.Entities;

public sealed class WeatherRecord
{
    public int Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public string RawJson { get; set; } = string.Empty;
}
