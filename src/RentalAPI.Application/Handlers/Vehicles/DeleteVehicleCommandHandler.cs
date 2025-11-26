using MediatR;
using RentalAPI.Application.Commands.Vehicles;
using RentalAPI.Domain.Interfaces;

namespace RentalAPI.Application.Handlers.Vehicles;

public class DeleteVehicleCommandHandler : IRequestHandler<DeleteVehicleCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;

    public DeleteVehicleCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(DeleteVehicleCommand request, CancellationToken cancellationToken)
    {
        var vehicle = await _unitOfWork.Vehicles.GetByIdAsync(request.Id);

        if (vehicle == null)
            return false;

        // Verify if vehicle has active rentals
        var activeRentals = await _unitOfWork.Rentals.GetByVehicleIdAsync(vehicle.Id);
        if (activeRentals.Any(r => r.Status == Domain.Enums.RentalStatus.Active))
            throw new InvalidOperationException("Cannot delete vehicle with active rentals.");

        await _unitOfWork.Vehicles.DeleteAsync(vehicle.Id);
        await _unitOfWork.SaveChangesAsync();

        return true;
    }
}
