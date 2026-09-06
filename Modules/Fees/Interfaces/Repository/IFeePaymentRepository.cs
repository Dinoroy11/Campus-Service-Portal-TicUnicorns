using CampusServicePortal.Modules.Fees.Entities;

namespace CampusServicePortal.Modules.Fees.Interfaces.Repository;

public interface IFeePaymentRepository
{
    Task<List<FeePayment>> GetAllAsync();

    Task<FeePayment?> GetByIdAsync(int feePaymentId);

    Task<List<FeePayment>> GetByStudentFeeIdAsync(int studentFeeId);

    Task<FeePayment> CreateAsync(FeePayment feePayment);

    Task<bool> ExistsAsync(int feePaymentId);

    Task<bool> ExistsForStudentFeeAsync(int studentFeeId);
}