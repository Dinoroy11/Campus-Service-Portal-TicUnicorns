using CampusServicePortal.Modules.Events.DTOs;
using FluentValidation;

namespace CampusServicePortal.Modules.Events.Validators;

public class UpdateEventValidator : AbstractValidator<UpdateEventDto>
{
    public UpdateEventValidator()
    {
        RuleFor(x => x.VenueId)
            .GreaterThan(0)
            .WithMessage("Venue is required.");

        RuleFor(x => x.EventName)
            .NotEmpty()
            .WithMessage("Event name is required.")
            .MaximumLength(150)
            .WithMessage("Event name cannot exceed 150 characters.");

        RuleFor(x => x.Description)
            .MaximumLength(1000)
            .When(x => x.Description != null)
            .WithMessage("Description cannot exceed 1000 characters.");

        RuleFor(x => x.StartDateTime)
            .NotEmpty()
            .WithMessage("Event start date and time is required.");

        RuleFor(x => x.EndDateTime)
            .NotEmpty()
            .WithMessage("Event end date and time is required.")
            .GreaterThan(x => x.StartDateTime)
            .WithMessage("Event end date and time must be after the start date and time.");

        RuleFor(x => x.FeeAmount)
            .GreaterThan(0)
            .When(x => x.IsPaid)
            .WithMessage("Paid events must have a fee greater than zero.");

        RuleFor(x => x.FeeAmount)
            .Must(fee => !fee.HasValue || fee.Value == 0)
            .When(x => !x.IsPaid)
            .WithMessage("Free events cannot have a fee amount.");

        RuleFor(x => x.HoldDurationMinutes)
            .GreaterThan(0)
            .WithMessage("Hold duration must be greater than zero.");
    }
}