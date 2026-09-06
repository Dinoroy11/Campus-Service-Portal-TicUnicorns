using CampusServicePortal.Modules.Labs.DTOs;
using FluentValidation;

namespace CampusServicePortal.Modules.Labs.Validators;

public class CreateLabSeatValidator
    : AbstractValidator<CreateLabSeatDto>
{
    public CreateLabSeatValidator()
    {
        RuleFor(x => x.LabId)
            .GreaterThan(0)
            .WithMessage("Valid lab is required.");

        RuleFor(x => x.SeatNumber)
            .NotEmpty()
            .WithMessage("Seat number is required.")
            .MaximumLength(50)
            .WithMessage(
                "Seat number cannot exceed 50 characters.");
    }
}