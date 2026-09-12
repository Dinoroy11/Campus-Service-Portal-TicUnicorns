using CampusServicePortal_TicUnicorns.Modules.Certificates.DTOs;
using CampusServicePortal_TicUnicorns.Modules.Certificates.Enums;
using FluentValidation;

namespace CampusServicePortal_TicUnicorns.Modules.Certificates.Validators;

public class UpdateCertificateValidator : AbstractValidator<UpdateCertificateDto>
{
    public UpdateCertificateValidator()
    {
        RuleFor(x => x.CertificateType)
            .NotEmpty()
            .WithMessage("Certificate type is required.")
            .Must(IsValidCertificateType)
            .WithMessage("Allowed certificate types: Bonafide, Transcript, CompletionLetter.");

        RuleFor(x => x.Purpose)
            .NotEmpty()
            .WithMessage("Purpose is required.")
            .MaximumLength(500)
            .WithMessage("Purpose cannot exceed 500 characters.");
    }

    private static bool IsValidCertificateType(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return false;

        var compact = value.Trim()
            .Replace(" ", string.Empty)
            .Replace("-", string.Empty);

        return Enum.TryParse<CertificateType>(compact, true, out _);
    }
}
