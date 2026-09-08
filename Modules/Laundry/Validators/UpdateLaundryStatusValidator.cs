using CampusServicePortal_TicUnicorns.Modules.Laundry.DTOs;
using FluentValidation;

namespace CampusServicePortal_TicUnicorns.Modules.Laundry.Validators
{
    public class UpdateLaundryStatusValidator
        : AbstractValidator<UpdateLaundryStatusDto>
    {
        public UpdateLaundryStatusValidator()
        {
            RuleFor(x => x.Status)
                .NotEmpty()
                .WithMessage("Status is required.")
                .Must(IsValidStatus)
                .WithMessage("Invalid laundry status.");
        }

        private bool IsValidStatus(string status)
        {
            return Enum.TryParse<Enums.LaundryStatus>(
                status,
                true,
                out _);
        }
    }
}