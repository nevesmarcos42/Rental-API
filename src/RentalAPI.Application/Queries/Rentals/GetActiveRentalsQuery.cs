using MediatR;
using RentalAPI.Application.DTOs;

namespace RentalAPI.Application.Queries.Rentals;

public class GetActiveRentalsQuery : IRequest<IEnumerable<RentalDto>>
{
}
