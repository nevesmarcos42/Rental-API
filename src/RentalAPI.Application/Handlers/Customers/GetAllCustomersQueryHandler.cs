using MediatR;
using RentalAPI.Application.DTOs;
using RentalAPI.Application.Queries.Customers;
using RentalAPI.Domain.Interfaces;

namespace RentalAPI.Application.Handlers.Customers;

public class GetAllCustomersQueryHandler : IRequestHandler<GetAllCustomersQuery, IEnumerable<CustomerDto>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetAllCustomersQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IEnumerable<CustomerDto>> Handle(GetAllCustomersQuery request, CancellationToken cancellationToken)
    {
        var customers = await _unitOfWork.Customers.GetAllAsync();

        return customers.Select(c => new CustomerDto
        {
            Id = c.Id,
            Name = c.Name,
            Email = c.Email,
            Phone = c.Phone,
            DriversLicense = c.DriversLicense,
            Address = c.Address
        }).ToList();
    }
}
