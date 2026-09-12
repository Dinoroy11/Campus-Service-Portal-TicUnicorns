using CampusServicePortal_TicUnicorns.Modules.Certificates.DTOs;
using CampusServicePortal_TicUnicorns.Modules.Certificates.Enums;
using FluentValidation;

namespace CampusServicePortal_TicUnicorns.Modules.Certificates.Validators;

public class UpdateCertificateStatusValidator
    : AbstractValidator<UpdateCertificateStatusDto>
{
    public UpdateCertificateStatusValidator()
    {
        RuleFor(x => x.Status)
            .NotEmpty()
            .WithMessage("Status is required.")
            .Must(IsValidStatus)
            .WithMessage("Allowed statuses: Pending, Approved, Rejected, Ready.");

        RuleFor(x => x.RejectionReason)
            .MaximumLength(500)
            .WithMessage("Rejection reason cannot exceed 500 characters.");

        RuleFor(x => x.DocumentPath)
            .MaximumLength(500)
            .WithMessage("Document path cannot exceed 500 characters.");
    }

    private static bool IsValidStatus(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return false;

        var compact = value.Trim()
            .Replace(" ", string.Empty)
            .Replace("-", string.Empty);

        return Enum.TryParse<CertificateStatus>(compact, true, out _);
    }
}
