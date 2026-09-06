namespace CampusServicePortal.Modules.Fees.DTOs;

public class FeeTypeDto
{
    public int FeeTypeId { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public decimal Amount { get; set; }

    public bool IsActive { get; set; }
}