using CampusServicePortal.Modules.Events.DTOs;
using CampusServicePortal.Modules.Events.Enums;
using FluentValidation;

namespace CampusServicePortal.Modules.Events.Validators;

public class EventRegistrationValidator
    : AbstractValidator<EventRegistrationDto>
{
    public EventRegistrationValidator()
    {
        RuleFor(x => x.EventId)
            .GreaterThan(0)
            .WithMessage("Event is required.");

        RuleFor(x => x.StudentId)
            .GreaterThan(0)
            .WithMessage("Student is required.");

        RuleFor(x => x.Status)
            .IsInEnum()
            .WithMessage("Invalid registration status.");

        RuleFor(x => x.RegisteredAt)
            .NotEmpty()
            .WithMessage(
                "Registration date and time is required.");
    }
}