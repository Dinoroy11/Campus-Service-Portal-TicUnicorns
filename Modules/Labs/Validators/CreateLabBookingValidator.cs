using CampusServicePortal.Modules.Labs.DTOs;
using FluentValidation;

namespace CampusServicePortal.Modules.Labs.Validators;

public class CreateLabBookingValidator
    : AbstractValidator<CreateLabBookingDto>
{
    public CreateLabBookingValidator()
    {
        RuleFor(x => x.LabId)
            .GreaterThan(0)
            .WithMessage("Valid lab is required.");

        RuleFor(x => x.TimeSlotId)
            .GreaterThan(0)
            .WithMessage("Valid time slot is required.");

        RuleFor(x => x.StudentId)
            .GreaterThan(0)
            .WithMessage("Valid student is required.");

        RuleFor(x => x.BookingDate)
            .NotEmpty()
            .WithMessage("Booking date is required.")
            .Must(date => date.Date >= DateTime.UtcNow.Date)
            .WithMessage(
                "Booking date cannot be in the past.");

        When(x => x.RequestedHours.HasValue, () =>
        {
            RuleFor(x => x.RequestedHours!.Value)
                .GreaterThan(0)
                .LessThanOrEqualTo(4)
                .WithMessage(
                    "Computer lab duration must be greater than 0 and cannot exceed 4 hours.");
        });
    }
}
