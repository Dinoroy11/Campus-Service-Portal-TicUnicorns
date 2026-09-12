namespace CampusServicePortal_TicUnicorns.Modules.Certificates.DTOs;

public class UpdateCertificateStatusDto
{
    // Admin transitions:
    // Pending -> Approved or Rejected
    // Approved -> Ready
    public string Status { get; set; } = string.Empty;

    // Required when Status = Rejected.
    public string? RejectionReason { get; set; }

    // Optional path/reference when Status = Ready.
    public string? DocumentPath { get; set; }
}
