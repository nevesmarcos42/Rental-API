using MediatR;
using RentalAPI.Application.DTOs;
using RentalAPI.Application.Queries.Rentals;
using RentalAPI.Domain.Interfaces;

namespace RentalAPI.Application.Handlers.Rentals;

public class GetRentalByIdQueryHandler : IRequestHandler<GetRentalByIdQuery, RentalDto?>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetRentalByIdQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<RentalDto?> Handle(GetRentalByIdQuery request, CancellationToken cancellationToken)
    {
        var rental = await _unitOfWork.Rentals.GetByIdAsync(request.Id);

        if (rental == null)
            return null;

        return new RentalDto
        {
            Id = rental.Id,
            CustomerId = rental.CustomerId,
            VehicleId = rental.VehicleId,
            StartDate = rental.StartDate,
            ExpectedEndDate = rental.ExpectedEndDate,
            ActualEndDate = rental.ActualEndDate,
            TotalAmount = rental.TotalAmount,
            Status = rental.Status
        };
    }
}
