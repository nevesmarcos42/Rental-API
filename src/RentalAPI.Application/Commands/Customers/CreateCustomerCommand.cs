using MediatR;
using RentalAPI.Application.DTOs;

namespace RentalAPI.Application.Commands.Customers;

public class CreateCustomerCommand : IRequest<CustomerDto>
{
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string DriversLicense { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public DateTime DateOfBirth { get; set; }
}
