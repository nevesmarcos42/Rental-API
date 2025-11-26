using MediatR;
using RentalAPI.Application.DTOs;

namespace RentalAPI.Application.Queries.Vehicles;

public class GetAvailableVehiclesQuery : IRequest<IEnumerable<VehicleDto>>
{
}
