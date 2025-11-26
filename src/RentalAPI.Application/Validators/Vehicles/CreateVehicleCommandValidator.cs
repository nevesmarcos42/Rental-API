using FluentValidation;
using RentalAPI.Application.Commands.Vehicles;

namespace RentalAPI.Application.Validators.Vehicles;

public class CreateVehicleCommandValidator : AbstractValidator<CreateVehicleCommand>
{
    public CreateVehicleCommandValidator()
    {
        RuleFor(x => x.Brand)
            .NotEmpty().WithMessage("Brand is required")
            .MaximumLength(100).WithMessage("Brand cannot exceed 100 characters");

        RuleFor(x => x.Model)
            .NotEmpty().WithMessage("Model is required")
            .MaximumLength(100).WithMessage("Model cannot exceed 100 characters");

        RuleFor(x => x.Year)
            .GreaterThan(1900).WithMessage("Year must be greater than 1900")
            .LessThanOrEqualTo(DateTime.Now.Year + 1).WithMessage("Year cannot be in the future");

        RuleFor(x => x.LicensePlate)
            .NotEmpty().WithMessage("License plate is required")
            .MaximumLength(20).WithMessage("License plate cannot exceed 20 characters");

        RuleFor(x => x.Color)
            .NotEmpty().WithMessage("Color is required")
            .MaximumLength(50).WithMessage("Color cannot exceed 50 characters");

        RuleFor(x => x.DailyRate)
            .GreaterThan(0).WithMessage("Daily rate must be greater than zero");

        RuleFor(x => x.Type)
            .IsInEnum().WithMessage("Invalid vehicle type");
    }
}
