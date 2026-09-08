namespace CampusServicePortal.Modules.Fees.DTOs;

public class CreateFeeTypeDto
{
    public string Name { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public decimal Amount { get; set; }
}