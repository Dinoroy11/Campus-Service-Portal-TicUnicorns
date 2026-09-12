using CampusServicePortal_TicUnicorns.Data;
using CampusServicePortal_TicUnicorns.Modules.Certificates.Entities;
using CampusServicePortal_TicUnicorns.Modules.Certificates.Interfaces.Repository;
using Microsoft.EntityFrameworkCore;

namespace CampusServicePortal_TicUnicorns.Modules.Certificates.Repositories;

public class CertificatesRepository : ICertificatesRepository
{
    private readonly CampusDbContext _context;

    public CertificatesRepository(CampusDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<CertificatesEntities>> GetAllAsync()
    {
        return await _context.Certificates
            .AsNoTracking()
            .OrderByDescending(x => x.RequestedAt)
            .ToListAsync();
    }

    public async Task<CertificatesEntities?> GetByIdAsync(int certificateId)
    {
        return await _context.Certificates
            .FirstOrDefaultAsync(x => x.CertificateId == certificateId);
    }

    public async Task<IReadOnlyList<CertificatesEntities>> GetByStudentIdAsync(
        int studentId)
    {
        return await _context.Certificates
            .AsNoTracking()
            .Where(x => x.StudentId == studentId)
            .OrderByDescending(x => x.RequestedAt)
            .ToListAsync();
    }

    public async Task<bool> HasPendingRequestAsync(
        int studentId,
        string certificateType,
        int? excludeCertificateId = null)
    {
        var query = _context.Certificates
            .AsNoTracking()
            .Where(x =>
                x.StudentId == studentId &&
                x.CertificateType == certificateType &&
                x.Status == "Pending");

        if (excludeCertificateId.HasValue)
        {
            query = query.Where(x =>
                x.CertificateId != excludeCertificateId.Value);
        }

        return await query.AnyAsync();
    }

    public async Task<CertificatesEntities> CreateAsync(
        CertificatesEntities certificate)
    {
        await _context.Certificates.AddAsync(certificate);
        await _context.SaveChangesAsync();
        return certificate;
    }

    public async Task UpdateAsync(CertificatesEntities certificate)
    {
        _context.Certificates.Update(certificate);
        await _context.SaveChangesAsync();
    }
}
