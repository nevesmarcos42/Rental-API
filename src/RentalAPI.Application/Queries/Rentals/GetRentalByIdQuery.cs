using MediatR;
using RentalAPI.Application.DTOs;

namespace RentalAPI.Application.Queries.Rentals;

public class GetRentalByIdQuery : IRequest<RentalDto?>
{
    public Guid Id { get; set; }
}
