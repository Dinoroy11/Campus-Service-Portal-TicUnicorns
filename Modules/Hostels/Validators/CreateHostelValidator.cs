using CampusServicePortal.Modules.Hostels.DTOs;
using FluentValidation;

namespace CampusServicePortal.Modules.Hostels.Validators;

public class CreateHostelValidator : AbstractValidator<CreateHostelDto>
{
    public CreateHostelValidator()
    {
        RuleFor(x => x.UniversityId)
            .GreaterThan(0)
            .WithMessage("University is required.");

        RuleFor(x => x.HostelName)
            .NotEmpty()
            .WithMessage("Hostel name is required.")
            .MaximumLength(100)
            .WithMessage("Hostel name cannot exceed 100 characters.");

        RuleFor(x => x.HostelType)
            .NotEmpty()
            .WithMessage("Hostel type is required.")
            .MaximumLength(50)
            .WithMessage("Hostel type cannot exceed 50 characters.");

        RuleFor(x => x.Description)
            .MaximumLength(500)
            .WithMessage("Description cannot exceed 500 characters.");
    }
}