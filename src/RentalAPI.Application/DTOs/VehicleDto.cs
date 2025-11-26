using RentalAPI.Domain.Enums;

namespace RentalAPI.Application.DTOs;

public class VehicleDto
{
    public Guid Id { get; set; }
    public string Brand { get; set; } = string.Empty;
    public string Model { get; set; } = string.Empty;
    public int Year { get; set; }
    public string LicensePlate { get; set; } = string.Empty;
    public VehicleType Type { get; set; }
    public VehicleStatus Status { get; set; }
    public decimal DailyRate { get; set; }
    public int Mileage { get; set; }
    public string Color { get; set; } = string.Empty;
}
