using CampusServicePortal_TicUnicorns.Data;
using CampusServicePortal_TicUnicorns.Modules.Certificates.Entities;
using CampusServicePortal_TicUnicorns.Modules.Certificates.Interfaces.Repository;
using Microsoft.EntityFrameworkCore;

namespace CampusServicePortal_TicUnicorns.Modules.Certificates.Repositories
{
    public class CertificatesRepository : ICertificatesRepository
    {
        private readonly CampusDbContext _context;

        public CertificatesRepository(CampusDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<CertificatesEntities>> GetAllAsync()
        {
            return await _context.Certificates
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<CertificatesEntities?> GetByIdAsync(
            int certificateId)
        {
            return await _context.Certificates
                .FirstOrDefaultAsync(c =>
                    c.CertificateId == certificateId);
        }

        public async Task<IEnumerable<CertificatesEntities>>
            GetByStudentIdAsync(int studentId)
        {
            return await _context.Certificates
                .AsNoTracking()
                .Where(c => c.StudentId == studentId)
                .ToListAsync();
        }

        public async Task<CertificatesEntities> CreateAsync(
            CertificatesEntities certificate)
        {
            await _context.Certificates.AddAsync(certificate);
            await _context.SaveChangesAsync();

            return certificate;
        }

        public async Task<bool> UpdateAsync(
            CertificatesEntities certificate)
        {
            _context.Certificates.Update(certificate);

            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> UpdateStatusAsync(
            int certificateId,
            string status,
            string? rejectionReason)
        {
            var certificate =
                await _context.Certificates
                    .FirstOrDefaultAsync(c =>
                        c.CertificateId == certificateId);

            if (certificate == null)
                return false;

            certificate.Status = status;
            certificate.RejectionReason = rejectionReason;

            if (status == "Processing" || status == "Done")
            {
                certificate.ProcessedAt = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeleteAsync(int certificateId)
        {
            var certificate =
                await _context.Certificates
                    .FirstOrDefaultAsync(c =>
                        c.CertificateId == certificateId);

            if (certificate == null)
                return false;

            _context.Certificates.Remove(certificate);

            await _context.SaveChangesAsync();

            return true;
        }
    }
}