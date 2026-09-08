using CampusServicePortal_TicUnicorns.Modules.Certificates.DTOs;
using FluentValidation;

namespace CampusServicePortal_TicUnicorns.Modules.Certificates.Validators
{
    public class CreateCertificateValidator : AbstractValidator<CreateCertificateDto>
    {
        public CreateCertificateValidator()
        {
            RuleFor(x => x.StudentId)
                .GreaterThan(0)
                .WithMessage("Student ID must be greater than 0.");

            RuleFor(x => x.CertificateType)
                .NotEmpty()
                .WithMessage("Certificate type is required.")
                .Must(IsValidCertificateType)
                .WithMessage("Invalid certificate type.");

            RuleFor(x => x.Purpose)
                .NotEmpty()
                .WithMessage("Purpose is required.")
                .MaximumLength(500)
                .WithMessage("Purpose cannot exceed 500 characters.");
        }

        private bool IsValidCertificateType(string certificateType)
        {
            return Enum.TryParse<Enums.CertificateType>(
                certificateType,
                true,
                out _);
        }
    }
}