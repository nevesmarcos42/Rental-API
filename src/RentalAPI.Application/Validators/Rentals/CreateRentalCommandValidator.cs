using FluentValidation;
using RentalAPI.Application.Commands.Rentals;

namespace RentalAPI.Application.Validators.Rentals;

public class CreateRentalCommandValidator : AbstractValidator<CreateRentalCommand>
{
    public CreateRentalCommandValidator()
    {
        RuleFor(x => x.CustomerId)
            .NotEmpty().WithMessage("Customer ID is required");

        RuleFor(x => x.VehicleId)
            .NotEmpty().WithMessage("Vehicle ID is required");

        RuleFor(x => x.StartDate)
            .NotEmpty().WithMessage("Start date is required")
            .GreaterThanOrEqualTo(DateTime.Today).WithMessage("Start date cannot be in the past");

        RuleFor(x => x.ExpectedEndDate)
            .NotEmpty().WithMessage("Expected end date is required")
            .GreaterThan(x => x.StartDate).WithMessage("Expected end date must be after start date");

        RuleFor(x => x.InitialMileage)
            .GreaterThanOrEqualTo(0).WithMessage("Initial mileage cannot be negative");
    }
}
