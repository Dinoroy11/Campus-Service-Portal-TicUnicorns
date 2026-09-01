namespace CampusServicePortal_TicUnicorns.Modules.Certificates.DTOs
{
    public class UpdateCertificateStatusDto
    {
        public string Status { get; set; } = string.Empty;

        public string? RejectionReason { get; set; }
    }
}