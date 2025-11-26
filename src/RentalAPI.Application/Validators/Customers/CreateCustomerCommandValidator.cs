using FluentValidation;
using RentalAPI.Application.Commands.Customers;

namespace RentalAPI.Application.Validators.Customers;

public class CreateCustomerCommandValidator : AbstractValidator<CreateCustomerCommand>
{
    public CreateCustomerCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required")
            .MaximumLength(200).WithMessage("Name cannot exceed 200 characters");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required")
            .EmailAddress().WithMessage("Invalid email format")
            .MaximumLength(200).WithMessage("Email cannot exceed 200 characters");

        RuleFor(x => x.Phone)
            .NotEmpty().WithMessage("Phone is required")
            .MaximumLength(20).WithMessage("Phone cannot exceed 20 characters");

        RuleFor(x => x.DriversLicense)
            .NotEmpty().WithMessage("Drivers license is required")
            .MaximumLength(50).WithMessage("Drivers license cannot exceed 50 characters");

        RuleFor(x => x.Address)
            .NotEmpty().WithMessage("Address is required")
            .MaximumLength(500).WithMessage("Address cannot exceed 500 characters");
    }
}
