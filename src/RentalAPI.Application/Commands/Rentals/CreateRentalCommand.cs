using MediatR;
using RentalAPI.Application.DTOs;

namespace RentalAPI.Application.Commands.Rentals;

public class CreateRentalCommand : IRequest<RentalDto>
{
    public Guid CustomerId { get; set; }
    public Guid VehicleId { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime ExpectedEndDate { get; set; }
    public int InitialMileage { get; set; }
    public string? Notes { get; set; }
}
