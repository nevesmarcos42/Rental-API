using MediatR;
using RentalAPI.Application.DTOs;

namespace RentalAPI.Application.Queries.Customers;

public class GetCustomerByIdQuery : IRequest<CustomerDto?>
{
    public Guid Id { get; set; }
}
