using CampusServicePortal.Modules.Events.DTOs;
using FluentValidation;

namespace CampusServicePortal.Modules.Events.Validators;

public class EventRegistrationValidator : AbstractValidator<EventRegistrationDto>
{
    public EventRegistrationValidator()
    {
        RuleFor(x => x.EventId)
            .GreaterThan(0)
            .WithMessage("Event is required.");

        RuleFor(x => x.StudentId)
            .GreaterThan(0)
            .WithMessage("Student is required.");

        RuleFor(x => x.EventSeatId)
            .GreaterThan(0)
            .When(x => x.EventSeatId.HasValue)
            .WithMessage("Event seat must be greater than zero.");

        RuleFor(x => x.Status)
            .NotEmpty()
            .WithMessage("Registration status is required.")
            .MaximumLength(30)
            .WithMessage("Registration status cannot exceed 30 characters.");

        RuleFor(x => x.RegisteredAt)
            .NotEmpty()
            .WithMessage("Registration date and time is required.");

        RuleFor(x => x.ExpiresAt)
            .GreaterThan(x => x.HeldAt)
            .When(x => x.HeldAt.HasValue && x.ExpiresAt.HasValue)
            .WithMessage("Expiration time must be after the hold time.");
    }
}