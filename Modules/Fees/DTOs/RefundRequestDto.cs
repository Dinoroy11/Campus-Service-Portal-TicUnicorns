namespace CampusServicePortal.Modules.Fees.DTOs;

public class RefundRequestDto
{
    public int RefundRequestId { get; set; }

    public int PaymentId { get; set; }

    public string Reason { get; set; } = string.Empty;

    public decimal Amount { get; set; }

    public string Status { get; set; } = string.Empty;

    public DateTime RequestedAt { get; set; }

    public int? ReviewedByUserId { get; set; }

    public DateTime? ProcessedAt { get; set; }
}