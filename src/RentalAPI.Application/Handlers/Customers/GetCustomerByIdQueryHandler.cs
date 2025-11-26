using MediatR;
using RentalAPI.Application.DTOs;
using RentalAPI.Application.Queries.Customers;
using RentalAPI.Domain.Interfaces;

namespace RentalAPI.Application.Handlers.Customers;

public class GetCustomerByIdQueryHandler : IRequestHandler<GetCustomerByIdQuery, CustomerDto?>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetCustomerByIdQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<CustomerDto?> Handle(GetCustomerByIdQuery request, CancellationToken cancellationToken)
    {
        var customer = await _unitOfWork.Customers.GetByIdAsync(request.Id);

        if (customer == null)
            return null;

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
