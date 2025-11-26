using MediatR;
using RentalAPI.Application.Commands.Customers;
using RentalAPI.Application.DTOs;
using RentalAPI.Domain.Interfaces;

namespace RentalAPI.Application.Handlers.Customers;

public class UpdateCustomerCommandHandler : IRequestHandler<UpdateCustomerCommand, CustomerDto>
{
    private readonly IUnitOfWork _unitOfWork;

    public UpdateCustomerCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<CustomerDto> Handle(UpdateCustomerCommand request, CancellationToken cancellationToken)
    {
        var customer = await _unitOfWork.Customers.GetByIdAsync(request.Id);

        if (customer == null)
            throw new KeyNotFoundException($"Customer with ID {request.Id} not found.");

        // Verify if email is already in use by another customer
        if (customer.Email != request.Email)
        {
            var existingCustomer = await _unitOfWork.Customers.GetByEmailAsync(request.Email);
            if (existingCustomer != null)
                throw new InvalidOperationException($"Customer with email {request.Email} already exists.");
        }

        // Verify if drivers license is already in use by another customer
        if (customer.DriversLicense != request.DriversLicense)
        {
            var existingCustomer = await _unitOfWork.Customers.GetByDriversLicenseAsync(request.DriversLicense);
            if (existingCustomer != null)
                throw new InvalidOperationException($"Customer with drivers license {request.DriversLicense} already exists.");
        }

        customer.Name = request.Name;
        customer.Email = request.Email;
        customer.Phone = request.Phone;
        customer.DriversLicense = request.DriversLicense;
        customer.Address = request.Address;

        await _unitOfWork.SaveChangesAsync();

        return new CustomerDto
        {
            Id = customer.Id,
            Name = customer.Name,
            Email = customer.Email,
            Phone = customer.Phone,
            DriversLicense = customer.DriversLicense,
            Address = customer.Address
        };
    }
}
