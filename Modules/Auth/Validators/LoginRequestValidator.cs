using FluentValidation;
using CampusServicePortal_TicUnicorns.Modules.Auth.DTOs;

namespace CampusServicePortal_TicUnicorns.Modules.Auth.Validators;

public class LoginRequestValidator : AbstractValidator<LoginRequestDto>
{
    public LoginRequestValidator()
    {
        RuleFor(x => x.Username)
            .NotEmpty()
            .WithMessage("Username is required.");

        RuleFor(x => x.Password)
            .NotEmpty()
            .WithMessage("Password is required.");
    }
}