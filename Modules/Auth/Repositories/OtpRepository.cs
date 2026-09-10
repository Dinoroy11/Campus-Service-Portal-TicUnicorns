using CampusServicePortal.Modules.Auth.Entities;
using CampusServicePortal_TicUnicorns.Data;
using CampusServicePortal_TicUnicorns.Modules.Auth.Interfaces.Repository;
using Microsoft.EntityFrameworkCore;

namespace CampusServicePortal_TicUnicorns.Modules.Auth.Repositories;

public class OtpRepository : IOtpRepository
{
    private readonly CampusDbContext _context;

    public OtpRepository(CampusDbContext context)
    {
        _context = context;
    }

    public async Task<OtpVerification?> GetLatestAsync(int userId)
    {
        return await _context.OtpVerifications
            .Where(x => x.UserId == userId)
            .OrderByDescending(x => x.CreatedAt)
            .FirstOrDefaultAsync();
    }

    public async Task AddAsync(OtpVerification otp)
    {
        await _context.OtpVerifications.AddAsync(otp);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(OtpVerification otp)
    {
        _context.OtpVerifications.Update(otp);
        await _context.SaveChangesAsync();
    }
}