namespace CampusServicePortal.Modules.Events.Entities;

public class EventPayment
{
    public int EventPaymentId { get; set; }
    public int RegistrationId { get; set; }
    public decimal Amount { get; set; }
    public string PaymentStatus { get; set; } = string.Empty;
    public string? PaymentReference { get; set; }
    public DateTime? PaidAt { get; set; }
}