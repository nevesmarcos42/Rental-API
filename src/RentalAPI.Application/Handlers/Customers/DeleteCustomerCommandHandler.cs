using MediatR;
using RentalAPI.Application.Commands.Customers;
using RentalAPI.Domain.Interfaces;

namespace RentalAPI.Application.Handlers.Customers;

public class DeleteCustomerCommandHandler : IRequestHandler<DeleteCustomerCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;

    public DeleteCustomerCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(DeleteCustomerCommand request, CancellationToken cancellationToken)
    {
        var customer = await _unitOfWork.Customers.GetByIdAsync(request.Id);

        if (customer == null)
            return false;

        // Verify if customer has active rentals
        var activeRentals = await _unitOfWork.Rentals.GetByCustomerIdAsync(customer.Id);
        if (activeRentals.Any(r => r.Status == Domain.Enums.RentalStatus.Active))
            throw new InvalidOperationException("Cannot delete customer with active rentals.");

        await _unitOfWork.Customers.DeleteAsync(customer.Id);
        await _unitOfWork.SaveChangesAsync();

        return true;
    }
}
