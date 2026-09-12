namespace CampusServicePortal_TicUnicorns.Modules.Certificates.DTOs;

public class CreateCertificateDto
{
    // Allowed values: Bonafide, Transcript, CompletionLetter
    public string CertificateType { get; set; } = string.Empty;

    // Student's reason/purpose for requesting the certificate.
    public string Purpose { get; set; } = string.Empty;
}
