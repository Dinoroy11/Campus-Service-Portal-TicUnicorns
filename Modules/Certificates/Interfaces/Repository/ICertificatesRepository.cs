using CampusServicePortal_TicUnicorns.Modules.Certificates.Entities;

namespace CampusServicePortal_TicUnicorns.Modules.Certificates.Interfaces.Repository
{
    public interface ICertificatesRepository
    {
        Task<IEnumerable<CertificatesEntities>> GetAllAsync();

        Task<CertificatesEntities?> GetByIdAsync(int certificateId);

        Task<IEnumerable<CertificatesEntities>> GetByStudentIdAsync(int studentId);

        Task<CertificatesEntities> CreateAsync(
            CertificatesEntities certificate);

        Task<bool> UpdateAsync(
            CertificatesEntities certificate);

        Task<bool> UpdateStatusAsync(
            int certificateId,
            string status,
            string? rejectionReason);

        Task<bool> DeleteAsync(int certificateId);
    }
}