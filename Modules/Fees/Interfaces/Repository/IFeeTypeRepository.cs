using CampusServicePortal.Modules.Fees.Entities;

namespace CampusServicePortal.Modules.Fees.Interfaces.Repository;

public interface IFeeTypeRepository
{
    Task<List<FeeType>> GetAllAsync();

    Task<FeeType?> GetByIdAsync(int feeTypeId);

    Task<FeeType> CreateAsync(FeeType feeType);

    Task UpdateAsync(FeeType feeType);

    Task<bool> ExistsAsync(int feeTypeId);
}