using FluentValidation;
using CampusServicePortal_TicUnicorns.Modules.Auth.DTOs;

namespace CampusServicePortal_TicUnicorns.Modules.Auth.Validators;

public class VerifyOtpRequestValidator
    : AbstractValidator<VerifyOtpRequestDto>
{
    public VerifyOtpRequestValidator()
    {
        RuleFor(x => x.Username)
            .NotEmpty()
            .WithMessage("Username is required.");

        RuleFor(x => x.Otp)
            .NotEmpty()
            .Matches(@"^\d{6}$")
            .WithMessage("OTP must contain exactly 6 digits.");
    }
}