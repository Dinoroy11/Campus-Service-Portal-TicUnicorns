namespace CampusServicePortal.Modules.Fees.DTOs;

public class StudentFeeDto
{
    public int StudentFeeId { get; set; }

    public int StudentId { get; set; }

    public int FeeTypeId { get; set; }

    public decimal Amount { get; set; }

    public DateTime? DueDate { get; set; }

    public string Status { get; set; } = string.Empty;

    public string? ExamReference { get; set; }

    public DateTime CreatedAt { get; set; }
}