using CampusServicePortal_TicUnicorns.Modules.Certificates.Entities;

namespace CampusServicePortal_TicUnicorns.Modules.Certificates.Interfaces.Repository;

public interface ICertificatesRepository
{
    Task<IReadOnlyList<CertificatesEntities>> GetAllAsync();

    Task<CertificatesEntities?> GetByIdAsync(int certificateId);

    Task<IReadOnlyList<CertificatesEntities>> GetByStudentIdAsync(int studentId);

    Task<bool> HasPendingRequestAsync(
        int studentId,
        string certificateType,
        int? excludeCertificateId = null);

    Task<CertificatesEntities> CreateAsync(CertificatesEntities certificate);

    Task UpdateAsync(CertificatesEntities certificate);
}
