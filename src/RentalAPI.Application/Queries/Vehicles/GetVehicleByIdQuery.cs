using MediatR;
using RentalAPI.Application.DTOs;

namespace RentalAPI.Application.Queries.Vehicles;

public class GetVehicleByIdQuery : IRequest<VehicleDto?>
{
    public Guid Id { get; set; }
}
