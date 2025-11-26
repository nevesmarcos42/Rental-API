using MediatR;
using RentalAPI.Application.DTOs;

namespace RentalAPI.Application.Queries.Rentals;

public class GetAllRentalsQuery : IRequest<IEnumerable<RentalDto>>
{
}
