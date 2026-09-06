using CampusServicePortal.Modules.Fees.Entities;
using CampusServicePortal.Modules.Fees.Interfaces.Repository;
using CampusServicePortal_TicUnicorns.Data;
using Microsoft.EntityFrameworkCore;

namespace CampusServicePortal.Modules.Fees.Repositories;

public class FeePaymentRepository : IFeePaymentRepository
{
    private readonly CampusDbContext _context;

    public FeePaymentRepository(CampusDbContext context)
    {
        _context = context;
    }

    public async Task<List<FeePayment>> GetAllAsync()
    {
        return await _context.Set<FeePayment>()
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<FeePayment?> GetByIdAsync(int feePaymentId)
    {
        return await _context.Set<FeePayment>()
            .FirstOrDefaultAsync(x => x.FeePaymentId == feePaymentId);
    }

    public async Task<List<FeePayment>> GetByStudentFeeIdAsync(
        int studentFeeId)
    {
        return await _context.Set<FeePayment>()
            .AsNoTracking()
            .Where(x => x.StudentFeeId == studentFeeId)
            .ToListAsync();
    }

    public async Task<FeePayment> CreateAsync(FeePayment feePayment)
    {
        await _context.Set<FeePayment>()
            .AddAsync(feePayment);

        await _context.SaveChangesAsync();

        return feePayment;
    }

    public async Task<bool> ExistsAsync(int feePaymentId)
    {
        return await _context.Set<FeePayment>()
            .AnyAsync(x => x.FeePaymentId == feePaymentId);
    }

    public async Task<bool> ExistsForStudentFeeAsync(int studentFeeId)
    {
        return await _context.Set<FeePayment>()
            .AnyAsync(x => x.StudentFeeId == studentFeeId);
    }
}