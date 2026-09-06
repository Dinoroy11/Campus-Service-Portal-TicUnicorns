using CampusServicePortal.Modules.Fees.Entities;

namespace CampusServicePortal.Modules.Fees.Interfaces.Repository;

public interface IRefundRequestRepository
{
    Task<List<RefundRequest>> GetAllAsync();

    Task<RefundRequest?> GetByIdAsync(int refundRequestId);

    Task<RefundRequest?> GetByPaymentIdAsync(int paymentId);

    Task<RefundRequest> CreateAsync(RefundRequest refundRequest);

    Task UpdateAsync(RefundRequest refundRequest);

    Task<bool> ExistsAsync(int refundRequestId);

    Task<bool> ExistsForPaymentAsync(int paymentId);
}