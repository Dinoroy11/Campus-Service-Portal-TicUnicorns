using CampusServicePortal.Modules.Events.Enums;

namespace CampusServicePortal.Modules.Events.Entities;

public class EventPayment
{
    public int EventPaymentId { get; set; }

    public int RegistrationId { get; set; }

    public decimal Amount { get; set; }

    public EventPaymentStatus PaymentStatus { get; set; }
        = EventPaymentStatus.Pending;

    public string? PaymentReference { get; set; }

    public DateTime? PaidAt { get; set; }
}