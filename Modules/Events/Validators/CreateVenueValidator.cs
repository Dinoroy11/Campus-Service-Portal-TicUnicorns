using CampusServicePortal.Modules.Events.DTOs;
using FluentValidation;

namespace CampusServicePortal.Modules.Events.Validators;

public class CreateVenueValidator : AbstractValidator<CreateVenueDto>
{
    public CreateVenueValidator()
    {
        RuleFor(x => x.VenueName)
            .NotEmpty()
            .WithMessage("Venue name is required.")
            .MaximumLength(100)
            .WithMessage("Venue name cannot exceed 100 characters.");

        RuleFor(x => x.Capacity)
            .GreaterThan(0)
            .WithMessage("Venue capacity must be greater than zero.");

        RuleFor(x => x.Description)
            .MaximumLength(500)
            .When(x => x.Description != null)
            .WithMessage("Description cannot exceed 500 characters.");
    }
}