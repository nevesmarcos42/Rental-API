using MediatR;
using RentalAPI.Application.Commands.Vehicles;
using RentalAPI.Application.DTOs;
using RentalAPI.Domain.Entities;
using RentalAPI.Domain.Enums;
using RentalAPI.Domain.Interfaces;

namespace RentalAPI.Application.Handlers.Vehicles;

public class CreateVehicleCommandHandler : IRequestHandler<CreateVehicleCommand, VehicleDto>
{
    private readonly IUnitOfWork _unitOfWork;

    public CreateVehicleCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<VehicleDto> Handle(CreateVehicleCommand request, CancellationToken cancellationToken)
    {
        var existingVehicle = await _unitOfWork.Vehicles.GetByLicensePlateAsync(request.LicensePlate);
        if (existingVehicle != null)
        {
            throw new InvalidOperationException("Já existe um veículo cadastrado com esta placa.");
        }

        var vehicle = new Vehicle
        {
            Id = Guid.NewGuid(),
            Brand = request.Brand,
            Model = request.Model,
            Year = request.Year,
            LicensePlate = request.LicensePlate,
            Type = request.Type,
            Status = VehicleStatus.Available,
            DailyRate = request.DailyRate,
            Mileage = 0,
            Color = request.Color,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await _unitOfWork.Vehicles.AddAsync(vehicle);
        await _unitOfWork.SaveChangesAsync();

        return new VehicleDto
        {
            Id = vehicle.Id,
            Brand = vehicle.Brand,
            Model = vehicle.Model,
            Year = vehicle.Year,
            LicensePlate = vehicle.LicensePlate,
            Type = vehicle.Type,
            Status = vehicle.Status,
            DailyRate = vehicle.DailyRate,
            Mileage = vehicle.Mileage,
            Color = vehicle.Color
        };
    }
}
