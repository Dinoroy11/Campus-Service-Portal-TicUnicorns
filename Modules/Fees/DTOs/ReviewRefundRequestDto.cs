namespace CampusServicePortal.Modules.Fees.DTOs;

public class ReviewRefundRequestDto
{
    // Allowed values: Approved, Rejected
    public string Status { get; set; } = string.Empty;
}
