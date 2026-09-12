namespace CampusServicePortal.Modules.Hostels.DTOs;

public class HostelPaymentDto
{
    public int HostelAllocationId { get; set; }
    public int StudentFeeId { get; set; }
    public int? FeePaymentId { get; set; }
    public decimal Amount { get; set; }
    public string FeeStatus { get; set; } = string.Empty;
    public string PaymentStatus { get; set; } = string.Empty;
    public string? PaymentReference { get; set; }
    public DateTime? DueDate { get; set; }
    public DateTime? PaidAt { get; set; }
}
