using System.ComponentModel.DataAnnotations;

namespace Domain;

public class WeatherData
{
    public Guid Id { get; set; }

    [MaxLength(64)]
    public string? Name { get; set; } = default!;

    public int? WmoCode { get; set; }

    public double? AirTemperature { get; set; }

    public double? WindSpeed { get; set; }
    
    [MaxLength(64)]
    public string? WeatherPhenomenon { get; set; }
    
    public DateTime? OccurrenceTime { get; set; }
}