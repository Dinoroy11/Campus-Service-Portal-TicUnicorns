using CampusServicePortal.Modules.Fees.Entities;
using CampusServicePortal.Modules.Fees.Interfaces.Repository;
using CampusServicePortal_TicUnicorns.Data;
using Microsoft.EntityFrameworkCore;

namespace CampusServicePortal.Modules.Fees.Repositories;

public class RefundRequestRepository : IRefundRequestRepository
{
    private readonly CampusDbContext _context;

    public RefundRequestRepository(CampusDbContext context)
    {
        _context = context;
    }

    public async Task<List<RefundRequest>> GetAllAsync()
    {
        return await _context.Set<RefundRequest>()
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<RefundRequest?> GetByIdAsync(
        int refundRequestId)
    {
        return await _context.Set<RefundRequest>()
            .FirstOrDefaultAsync(
                x => x.RefundRequestId == refundRequestId);
    }

    public async Task<RefundRequest?> GetByPaymentIdAsync(
        int paymentId)
    {
        return await _context.Set<RefundRequest>()
            .FirstOrDefaultAsync(
                x => x.PaymentId == paymentId);
    }

    public async Task<RefundRequest> CreateAsync(
        RefundRequest refundRequest)
    {
        await _context.Set<RefundRequest>()
            .AddAsync(refundRequest);

        await _context.SaveChangesAsync();

        return refundRequest;
    }

    public async Task UpdateAsync(
        RefundRequest refundRequest)
    {
        _context.Set<RefundRequest>()
            .Update(refundRequest);

        await _context.SaveChangesAsync();
    }

    public async Task<bool> ExistsAsync(
        int refundRequestId)
    {
        return await _context.Set<RefundRequest>()
            .AnyAsync(x => x.RefundRequestId == refundRequestId);
    }

    public async Task<bool> ExistsForPaymentAsync(
        int paymentId)
    {
        return await _context.Set<RefundRequest>()
            .AnyAsync(x => x.PaymentId == paymentId);
    }
}