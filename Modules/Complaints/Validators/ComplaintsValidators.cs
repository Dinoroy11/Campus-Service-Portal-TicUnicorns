using CampusServicePortal.Modules.Complaints.DTOs;
using FluentValidation;

namespace CampusServicePortal.Modules.Complaints.Validators;

// =========================================================
// Create Complaint
// =========================================================

public class CreateComplaintValidator
    : AbstractValidator<CreateComplaintDto>
{
    public CreateComplaintValidator()
    {
        RuleFor(x => x.CategoryId)
            .GreaterThan(0)
            .WithMessage("CategoryId must be greater than 0.");

        RuleFor(x => x.StudentId)
            .GreaterThan(0)
            .WithMessage("StudentId must be greater than 0.");

        RuleFor(x => x.Title)
            .NotEmpty()
            .WithMessage("Complaint title is required.")
            .MaximumLength(200)
            .WithMessage("Complaint title cannot exceed 200 characters.");

        RuleFor(x => x.Description)
            .NotEmpty()
            .WithMessage("Complaint description is required.")
            .MaximumLength(2000)
            .WithMessage("Complaint description cannot exceed 2000 characters.");
    }
}

// =========================================================
// Create Complaint Category
// =========================================================

public class CreateComplaintCategoryValidator
    : AbstractValidator<CreateComplaintCategoryDto>
{
    public CreateComplaintCategoryValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Category name is required.")
            .MaximumLength(100)
            .WithMessage("Category name cannot exceed 100 characters.");

        RuleFor(x => x.Description)
            .MaximumLength(500)
            .WithMessage("Category description cannot exceed 500 characters.");
    }
}

// =========================================================
// Update Complaint
// =========================================================

public class UpdateComplaintValidator
    : AbstractValidator<UpdateComplaintDto>
{
    private static readonly string[] AllowedStatuses =
    {
        "Submitted",
        "UnderReview",
        "InProgress",
        "Resolved",
        "Rejected",
        "Closed"
    };

    public UpdateComplaintValidator()
    {
        RuleFor(x => x.Status)
            .NotEmpty()
            .WithMessage("Complaint status is required.")
            .Must(status => AllowedStatuses.Contains(status))
            .WithMessage("Invalid complaint status.");

        RuleFor(x => x.ActionRemarks)
            .MaximumLength(2000)
            .WithMessage("Action remarks cannot exceed 2000 characters.");

        RuleFor(x => x.StatusChangedBy)
            .GreaterThan(0)
            .When(x => x.StatusChangedBy.HasValue)
            .WithMessage("StatusChangedBy must be greater than 0.");
    }
}