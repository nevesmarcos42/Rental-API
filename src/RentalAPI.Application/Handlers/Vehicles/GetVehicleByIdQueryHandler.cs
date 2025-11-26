using MediatR;
using RentalAPI.Application.DTOs;
using RentalAPI.Application.Queries.Vehicles;
using RentalAPI.Domain.Interfaces;

namespace RentalAPI.Application.Handlers.Vehicles;

public class GetVehicleByIdQueryHandler : IRequestHandler<GetVehicleByIdQuery, VehicleDto?>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetVehicleByIdQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<VehicleDto?> Handle(GetVehicleByIdQuery request, CancellationToken cancellationToken)
    {
        var vehicle = await _unitOfWork.Vehicles.GetByIdAsync(request.Id);

        if (vehicle == null)
            return null;

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
