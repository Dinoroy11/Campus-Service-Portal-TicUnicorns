namespace CampusServicePortal.Modules.Fees.Entities;

public class FeeType
{
    public int FeeTypeId { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public decimal Amount { get; set; }

    public bool IsActive { get; set; } = true;

    public ICollection<StudentFee> StudentFees { get; set; }
        = new List<StudentFee>();
}