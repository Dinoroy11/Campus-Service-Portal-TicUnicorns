using CampusServicePortal_TicUnicorns.Modules.Certificates.DTOs;
using FluentValidation;

namespace CampusServicePortal_TicUnicorns.Modules.Certificates.Validators
{
    public class UpdateCertificateStatusValidator
        : AbstractValidator<UpdateCertificateStatusDto>
    {
        public UpdateCertificateStatusValidator()
        {
            RuleFor(x => x.Status)
                .NotEmpty()
                .WithMessage("Status is required.")
                .Must(IsValidStatus)
                .WithMessage("Invalid certificate status.");

            RuleFor(x => x.RejectionReason)
                .MaximumLength(500)
                .WithMessage("Rejection reason cannot exceed 500 characters.");
        }

        private bool IsValidStatus(string status)
        {
            return Enum.TryParse<Enums.CertificateStatus>(
                status,
                true,
                out _);
        }
    }
}