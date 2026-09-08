using CampusServicePortal_TicUnicorns.Modules.Laundry.DTOs;
using FluentValidation;

namespace CampusServicePortal_TicUnicorns.Modules.Laundry.Validators
{
    public class UpdateLaundryValidator : AbstractValidator<UpdateLaundryDto>
    {
        public UpdateLaundryValidator()
        {
            RuleFor(x => x.ServiceType)
                .NotEmpty()
                .WithMessage("Service type is required.")
                .Must(IsValidServiceType)
                .WithMessage("Invalid laundry service type.");

            RuleFor(x => x.Quantity)
                .GreaterThan(0)
                .WithMessage("Quantity must be greater than 0.");

            RuleFor(x => x.PickupMethod)
                .NotEmpty()
                .WithMessage("Pickup method is required.");
        }

        private bool IsValidServiceType(string serviceType)
        {
            return Enum.TryParse<Enums.LaundryServiceType>(
                serviceType,
                true,
                out _);
        }
    }
}