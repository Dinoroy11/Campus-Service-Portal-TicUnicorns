using FluentValidation;
using CampusServicePortal_TicUnicorns.Modules.Auth.DTOs;

namespace CampusServicePortal_TicUnicorns.Modules.Auth.Validators;

public class ResetPasswordRequestValidator
    : AbstractValidator<ResetPasswordRequestDto>
{
    public ResetPasswordRequestValidator()
    {
        RuleFor(x => x.Token)
            .NotEmpty()
            .WithMessage("Reset token is required.");

        RuleFor(x => x.NewPassword)
            .NotEmpty()
            .MinimumLength(8)
            .WithMessage("New password must be at least 8 characters.");

        RuleFor(x => x.ConfirmNewPassword)
            .Equal(x => x.NewPassword)
            .WithMessage("Passwords do not match.");
    }
}