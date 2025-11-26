using MediatR;
using RentalAPI.Application.DTOs;

namespace RentalAPI.Application.Queries.Customers;

public class GetAllCustomersQuery : IRequest<IEnumerable<CustomerDto>>
{
}
