using CampusServicePortal.Modules.Labs.DTOs;
using FluentValidation;

namespace CampusServicePortal.Modules.Labs.Validators;

public class CreateLabValidator : AbstractValidator<CreateLabDto>
{
    public CreateLabValidator()
    {
        RuleFor(x => x.LabName)
            .NotEmpty()
            .WithMessage("Lab name is required.")
            .MaximumLength(150)
            .WithMessage("Lab name cannot exceed 150 characters.");

        RuleFor(x => x.LabType)
            .NotEmpty()
            .WithMessage("Lab type is required.")
            .Must(type =>
                type.Equals("Science", StringComparison.OrdinalIgnoreCase) ||
                type.Equals("Computer", StringComparison.OrdinalIgnoreCase))
            .WithMessage("Lab type must be Science or Computer.");

        RuleFor(x => x.Capacity)
            .GreaterThan(0)
            .WithMessage("Lab capacity must be greater than zero.");

        RuleFor(x => x.Description)
            .MaximumLength(500)
            .WithMessage(
                "Description cannot exceed 500 characters.");
    }
}
