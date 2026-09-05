using CampusServicePortal_TicUnicorns.Modules.Certificates.Entities;
using CampusServicePortal_TicUnicorns.Modules.Certificates.Interfaces.Repository;

namespace CampusServicePortal_TicUnicorns.Modules.Certificates.Repositories
{
    public class CertificatesRepository : ICertificatesRepository
    {
        public Task<IEnumerable<CertificatesEntities>> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public Task<CertificatesEntities?> GetByIdAsync(int certificateId)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<CertificatesEntities>> GetByStudentIdAsync(
            int studentId)
        {
            throw new NotImplementedException();
        }

        public Task<CertificatesEntities> CreateAsync(
            CertificatesEntities certificate)
        {
            throw new NotImplementedException();
        }

        public Task<bool> UpdateAsync(
            CertificatesEntities certificate)
        {
            throw new NotImplementedException();
        }

        public Task<bool> UpdateStatusAsync(
            int certificateId,
            string status,
            string? rejectionReason)
        {
            throw new NotImplementedException();
        }

        public Task<bool> AssignAsync(
            int certificateId,
            int assignedTo)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteAsync(int certificateId)
        {
            throw new NotImplementedException();
        }
    }
}