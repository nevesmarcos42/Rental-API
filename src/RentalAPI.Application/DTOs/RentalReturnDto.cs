using RentalAPI.Domain.Enums;

namespace RentalAPI.Application.DTOs;

public class RentalReturnDto
{
    public Guid Id { get; set; }
    public Guid RentalId { get; set; }
    public DateTime ReturnDate { get; set; }
    public VehicleCondition Condition { get; set; }
    public decimal LateFee { get; set; }
    public decimal DamageFee { get; set; }
    public string? Notes { get; set; }
    public bool InspectionApproved { get; set; }
    public string InspectedBy { get; set; } = string.Empty;
}
