using MediatR;

namespace RentalAPI.Application.Commands.Customers;

public class DeleteCustomerCommand : IRequest<bool>
{
    public Guid Id { get; set; }
}
