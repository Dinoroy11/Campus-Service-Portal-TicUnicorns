using CampusServicePortal.Modules.Events.DTOs;
using FluentValidation;

namespace CampusServicePortal.Modules.Events.Validators;

public class EventPaymentValidator : AbstractValidator<EventPaymentDto>
{
    public EventPaymentValidator()
    {
        RuleFor(x => x.RegistrationId)
            .GreaterThan(0)
            .WithMessage("Event registration is required.");

        RuleFor(x => x.Amount)
            .GreaterThan(0)
            .WithMessage("Payment amount must be greater than zero.");

        RuleFor(x => x.PaymentStatus)
            .IsInEnum()
            .WithMessage("Invalid payment status.");

        RuleFor(x => x.PaymentReference)
            .MaximumLength(100)
            .When(x => x.PaymentReference != null)
            .WithMessage("Payment reference cannot exceed 100 characters.");

        RuleFor(x => x.PaidAt)
            .NotEmpty()
            .When(x => x.PaymentStatus ==
                       Modules.Events.Enums.EventPaymentStatus.Paid)
            .WithMessage(
                "Paid date and time is required when payment status is Paid.");
    }
}