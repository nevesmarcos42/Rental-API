using MediatR;
using RentalAPI.Application.Commands.Customers;
using RentalAPI.Application.DTOs;
using RentalAPI.Domain.Entities;
using RentalAPI.Domain.Interfaces;

namespace RentalAPI.Application.Handlers.Customers;

public class CreateCustomerCommandHandler : IRequestHandler<CreateCustomerCommand, CustomerDto>
{
    private readonly IUnitOfWork _unitOfWork;

    public CreateCustomerCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<CustomerDto> Handle(CreateCustomerCommand request, CancellationToken cancellationToken)
    {
        var existingEmail = await _unitOfWork.Customers.GetByEmailAsync(request.Email);
        if (existingEmail != null)
        {
            throw new InvalidOperationException("Já existe um cliente cadastrado com este email.");
        }

        var existingLicense = await _unitOfWork.Customers.GetByDriversLicenseAsync(request.DriversLicense);
        if (existingLicense != null)
        {
            throw new InvalidOperationException("Já existe um cliente cadastrado com esta CNH.");
        }

        var customer = new Customer
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Email = request.Email,
            Phone = request.Phone,
            DriversLicense = request.DriversLicense,
            Address = request.Address,
            DateOfBirth = request.DateOfBirth,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await _unitOfWork.Customers.AddAsync(customer);
        await _unitOfWork.SaveChangesAsync();

        return new CustomerDto
        {
            Id = customer.Id,
            Name = customer.Name,
            Email = customer.Email,
            Phone = customer.Phone,
            DriversLicense = customer.DriversLicense,
            Address = customer.Address,
            DateOfBirth = customer.DateOfBirth,
            IsActive = customer.IsActive
        };
    }
}
