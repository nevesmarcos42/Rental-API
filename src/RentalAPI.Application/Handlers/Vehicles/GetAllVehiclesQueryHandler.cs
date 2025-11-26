using MediatR;
using RentalAPI.Application.DTOs;
using RentalAPI.Application.Queries.Vehicles;
using RentalAPI.Domain.Interfaces;

namespace RentalAPI.Application.Handlers.Vehicles;

public class GetAllVehiclesQueryHandler : IRequestHandler<GetAllVehiclesQuery, IEnumerable<VehicleDto>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetAllVehiclesQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IEnumerable<VehicleDto>> Handle(GetAllVehiclesQuery request, CancellationToken cancellationToken)
    {
        var vehicles = await _unitOfWork.Vehicles.GetAllAsync();

        return vehicles.Select(v => new VehicleDto
        {
            Id = v.Id,
            Brand = v.Brand,
            Model = v.Model,
            Year = v.Year,
            LicensePlate = v.LicensePlate,
            Type = v.Type,
            Status = v.Status,
            DailyRate = v.DailyRate,
            Mileage = v.Mileage,
            Color = v.Color
        });
    }
}
