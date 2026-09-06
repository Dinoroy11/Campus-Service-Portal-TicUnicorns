namespace CampusServicePortal.Modules.Fees.Entities;

public class FeePayment
{
    public int FeePaymentId { get; set; }

    public int StudentFeeId { get; set; }

    public decimal Amount { get; set; }

    public string PaymentStatus { get; set; } = string.Empty;

    public string? PaymentReference { get; set; }

    public DateTime? PaidAt { get; set; }

    public int? SourcePaymentId { get; set; }

    public int? SourceFeeId { get; set; }

    public int? TargetStudentFeeId { get; set; }

    public StudentFee StudentFee { get; set; } = null!;

    public RefundRequest? RefundRequest { get; set; }
}