using RentalAPI.Domain.Enums;

namespace RentalAPI.Application.DTOs;

public class RentalDto
{
    public Guid Id { get; set; }
    public Guid CustomerId { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public Guid VehicleId { get; set; }
    public string VehicleDescription { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime ExpectedEndDate { get; set; }
    public DateTime? ActualEndDate { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal DailyRate { get; set; }
    public RentalStatus Status { get; set; }
    public int InitialMileage { get; set; }
    public int? FinalMileage { get; set; }
    public string? Notes { get; set; }
}
