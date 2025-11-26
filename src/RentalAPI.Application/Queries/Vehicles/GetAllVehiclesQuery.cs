using MediatR;
using RentalAPI.Application.DTOs;

namespace RentalAPI.Application.Queries.Vehicles;

public class GetAllVehiclesQuery : IRequest<IEnumerable<VehicleDto>>
{
}
