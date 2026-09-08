using CampusServicePortal.Modules.Fees.Entities;
using CampusServicePortal.Modules.Fees.Interfaces.Repository;
using CampusServicePortal_TicUnicorns.Data;
using Microsoft.EntityFrameworkCore;

namespace CampusServicePortal.Modules.Fees.Repositories;

public class FeeTypeRepository : IFeeTypeRepository
{
    private readonly CampusDbContext _context;

    public FeeTypeRepository(CampusDbContext context)
    {
        _context = context;
    }

    public async Task<List<FeeType>> GetAllAsync()
    {
        return await _context.Set<FeeType>()
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<FeeType?> GetByIdAsync(int feeTypeId)
    {
        return await _context.Set<FeeType>()
            .FirstOrDefaultAsync(x => x.FeeTypeId == feeTypeId);
    }

    public async Task<FeeType> CreateAsync(FeeType feeType)
    {
        await _context.Set<FeeType>()
            .AddAsync(feeType);

        await _context.SaveChangesAsync();

        return feeType;
    }

    public async Task UpdateAsync(FeeType feeType)
    {
        _context.Set<FeeType>()
            .Update(feeType);

        await _context.SaveChangesAsync();
    }

    public async Task<bool> ExistsAsync(int feeTypeId)
    {
        return await _context.Set<FeeType>()
            .AnyAsync(x => x.FeeTypeId == feeTypeId);
    }
}