namespace CampusServicePortal_TicUnicorns.Modules.Certificates.DTOs;

public class UpdateCertificateDto
{
    // Can be changed by the owner only while status is Pending.
    public string CertificateType { get; set; } = string.Empty;

    public string Purpose { get; set; } = string.Empty;
}
