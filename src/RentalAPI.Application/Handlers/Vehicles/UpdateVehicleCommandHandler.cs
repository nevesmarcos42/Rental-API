using MediatR;
using RentalAPI.Application.Commands.Vehicles;
using RentalAPI.Application.DTOs;
using RentalAPI.Domain.Interfaces;

namespace RentalAPI.Application.Handlers.Vehicles;

public class UpdateVehicleCommandHandler : IRequestHandler<UpdateVehicleCommand, VehicleDto>
{
    private readonly IUnitOfWork _unitOfWork;

    public UpdateVehicleCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<VehicleDto> Handle(UpdateVehicleCommand request, CancellationToken cancellationToken)
    {
        var vehicle = await _unitOfWork.Vehicles.GetByIdAsync(request.Id);

        if (vehicle == null)
            throw new KeyNotFoundException($"Vehicle with ID {request.Id} not found.");

        // Verify if license plate is already in use by another vehicle
        if (vehicle.LicensePlate != request.LicensePlate)
        {
            var existingVehicle = await _unitOfWork.Vehicles.GetByLicensePlateAsync(request.LicensePlate);
            if (existingVehicle != null)
                throw new InvalidOperationException($"Vehicle with license plate {request.LicensePlate} already exists.");
        }

        vehicle.LicensePlate = request.LicensePlate;
        vehicle.Brand = request.Brand;
        vehicle.Model = request.Model;
        vehicle.Year = request.Year;
        vehicle.Color = request.Color;
        vehicle.Type = request.Type;
        vehicle.DailyRate = request.DailyRate;
        vehicle.Status = request.Status;

        await _unitOfWork.SaveChangesAsync();

        return new VehicleDto
        {
            Id = vehicle.Id,
            LicensePlate = vehicle.LicensePlate,
            Brand = vehicle.Brand,
            Model = vehicle.Model,
            Year = vehicle.Year,
            Color = vehicle.Color,
            Type = vehicle.Type,
            DailyRate = vehicle.DailyRate,
            Status = vehicle.Status
        };
    }
}
