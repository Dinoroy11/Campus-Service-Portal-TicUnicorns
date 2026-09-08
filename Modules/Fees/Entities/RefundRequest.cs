using CampusServicePortal_TicUnicorns.Modules.Fees.Enums;

namespace CampusServicePortal.Modules.Fees.Entities;

public class RefundRequest
{
    public int RefundRequestId { get; set; }

    public int PaymentId { get; set; }

    public string Reason { get; set; } = string.Empty;

    public decimal Amount { get; set; }

    public RefundStatus Status { get; set; } = RefundStatus.Pending;

    public DateTime RequestedAt { get; set; }

    public int? ReviewedByUserId { get; set; }

    public DateTime? ProcessedAt { get; set; }

    public FeePayment Payment { get; set; } = null!;
}