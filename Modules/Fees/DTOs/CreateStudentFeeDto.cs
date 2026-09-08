namespace CampusServicePortal.Modules.Fees.DTOs;

public class CreateStudentFeeDto
{
    public int StudentId { get; set; }

    public int FeeTypeId { get; set; }

    public decimal Amount { get; set; }

    public DateTime? DueDate { get; set; }

    public string? ExamReference { get; set; }
}