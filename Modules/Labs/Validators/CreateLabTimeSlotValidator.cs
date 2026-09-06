using CampusServicePortal.Modules.Labs.DTOs;
using FluentValidation;

namespace CampusServicePortal.Modules.Labs.Validators;

public class CreateLabTimeSlotValidator
    : AbstractValidator<CreateLabTimeSlotDto>
{
    public CreateLabTimeSlotValidator()
    {
        RuleFor(x => x.LabId)
            .GreaterThan(0)
            .WithMessage("Valid lab is required.");

        RuleFor(x => x.EndTime)
            .GreaterThan(x => x.StartTime)
            .WithMessage(
                "End time must be later than start time.");
    }
}