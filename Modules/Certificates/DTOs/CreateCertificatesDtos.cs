namespace CampusServicePortal_TicUnicorns.Modules.Certificates.DTOs
{
    public class CreateCertificateDto
    {
        public int StudentId { get; set; }

        public string CertificateType { get; set; } = string.Empty;

        public string Purpose { get; set; } = string.Empty;
    }
}