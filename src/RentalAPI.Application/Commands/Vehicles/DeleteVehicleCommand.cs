using MediatR;

namespace RentalAPI.Application.Commands.Vehicles;

public class DeleteVehicleCommand : IRequest<bool>
{
    public Guid Id { get; set; }
}
