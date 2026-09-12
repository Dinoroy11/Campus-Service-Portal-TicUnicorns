using CampusServicePortal.Modules.Complaints.DTOs;
using FluentValidation;

namespace CampusServicePortal.Modules.Complaints.Validators;

public class CreateComplaintValidator
    : AbstractValidator<CreateComplaintDto>
{
    public CreateComplaintValidator()
    {
        RuleFor(x => x.CategoryId)
            .GreaterThan(0)
            .WithMessage("CategoryId must be greater than 0.");

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

public class UpdateComplaintValidator
    : AbstractValidator<UpdateComplaintDto>
{
    private static readonly string[] AllowedStatuses =
    {
        "UnderReview",
        "InProgress",
        "Resolved",
        "Rejected"
    };

    public UpdateComplaintValidator()
    {
        RuleFor(x => x.Status)
            .NotEmpty()
            .WithMessage("Complaint status is required.")
            .Must(status => AllowedStatuses.Contains(status))
            .WithMessage(
                "Status must be UnderReview, InProgress, Resolved, or Rejected.");

        RuleFor(x => x.ActionRemarks)
            .MaximumLength(2000)
            .WithMessage("Action remarks cannot exceed 2000 characters.");
    }
}

public class ConfirmComplaintResolutionValidator
    : AbstractValidator<ConfirmComplaintResolutionDto>
{
    public ConfirmComplaintResolutionValidator()
    {
        RuleFor(x => x.Remarks)
            .MaximumLength(2000)
            .WithMessage("Remarks cannot exceed 2000 characters.");
    }
}
