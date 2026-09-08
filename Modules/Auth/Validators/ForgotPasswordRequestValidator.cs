using FluentValidation;
using CampusServicePortal_TicUnicorns.Modules.Auth.DTOs;

namespace CampusServicePortal_TicUnicorns.Modules.Auth.Validators;

public class ForgotPasswordRequestValidator
    : AbstractValidator<ForgotPasswordRequestDto>
{
    public ForgotPasswordRequestValidator()
    {
        RuleFor(x => x.Username)
            .NotEmpty()
            .WithMessage("Username is required.");
    }
}