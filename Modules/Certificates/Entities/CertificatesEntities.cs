namespace CampusServicePortal_TicUnicorns.Modules.Certificates.Entities
{
    public class CertificatesEntities
    {
        public int CertificateId { get; set; }

        public int StudentId { get; set; }

        public string CertificateType { get; set; } = string.Empty;

        public string Purpose { get; set; } = string.Empty;

        public string Status { get; set; } = string.Empty;

        public DateTime RequestedAt { get; set; }

        public DateTime? ProcessedAt { get; set; }

        public DateTime? IssuedAt { get; set; }

        public int? AssignedTo { get; set; }

        public string? RejectionReason { get; set; }

        public string? DocumentPath { get; set; }
    }
}