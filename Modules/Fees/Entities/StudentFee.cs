namespace CampusServicePortal.Modules.Fees.Entities;

public class StudentFee
{
    public int StudentFeeId { get; set; }

    public int StudentId { get; set; }

    public int FeeTypeId { get; set; }

    public decimal Amount { get; set; }

    public DateTime? DueDate { get; set; }

    public string Status { get; set; } = string.Empty;

    public string? ExamReference { get; set; }

    public DateTime CreatedAt { get; set; }

    public FeeType FeeType { get; set; } = null!;

    public ICollection<FeePayment> FeePayments { get; set; }
        = new List<FeePayment>();
}