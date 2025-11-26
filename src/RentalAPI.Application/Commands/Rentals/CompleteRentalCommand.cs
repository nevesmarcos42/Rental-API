using MediatR;
using RentalAPI.Application.DTOs;
using RentalAPI.Domain.Enums;

namespace RentalAPI.Application.Commands.Rentals;

public class CompleteRentalCommand : IRequest<RentalReturnDto>
{
    public Guid RentalId { get; set; }
    public DateTime ReturnDate { get; set; }
    public int FinalMileage { get; set; }
    public VehicleCondition Condition { get; set; }
    public string? Notes { get; set; }
    public string InspectedBy { get; set; } = string.Empty;
}
