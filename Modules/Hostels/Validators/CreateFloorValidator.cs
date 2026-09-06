using CampusServicePortal.Modules.Hostels.DTOs;
using FluentValidation;

namespace CampusServicePortal.Modules.Hostels.Validators;

public class CreateFloorValidator : AbstractValidator<CreateFloorDto>
{
    public CreateFloorValidator()
    {
        RuleFor(x => x.HostelId)
            .GreaterThan(0)
            .WithMessage("Hostel is required.");

        RuleFor(x => x.FloorNumber)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Floor number cannot be negative.");

        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Floor name is required.")
            .MaximumLength(100)
            .WithMessage("Floor name cannot exceed 100 characters.");
    }
}