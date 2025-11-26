using MediatR;
using RentalAPI.Application.DTOs;
using RentalAPI.Application.Queries.Rentals;
using RentalAPI.Domain.Interfaces;

namespace RentalAPI.Application.Handlers.Rentals;

public class GetAllRentalsQueryHandler : IRequestHandler<GetAllRentalsQuery, IEnumerable<RentalDto>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetAllRentalsQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IEnumerable<RentalDto>> Handle(GetAllRentalsQuery request, CancellationToken cancellationToken)
    {
        var rentals = await _unitOfWork.Rentals.GetAllAsync();

        return rentals.Select(r => new RentalDto
        {
            Id = r.Id,
            CustomerId = r.CustomerId,
            VehicleId = r.VehicleId,
            StartDate = r.StartDate,
            ExpectedEndDate = r.ExpectedEndDate,
            ActualEndDate = r.ActualEndDate,
            TotalAmount = r.TotalAmount,
            Status = r.Status
        }).ToList();
    }
}
