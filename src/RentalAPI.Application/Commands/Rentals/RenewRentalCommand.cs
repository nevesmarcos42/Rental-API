using MediatR;
using RentalAPI.Application.DTOs;

namespace RentalAPI.Application.Commands.Rentals;

public class RenewRentalCommand : IRequest<RentalDto>
{
    public Guid RentalId { get; set; }
    public DateTime NewExpectedEndDate { get; set; }
}
