using CampusServicePortal.Modules.Fees.DTOs;

namespace CampusServicePortal.Modules.Fees.Interfaces.Service;

public interface IFeesService
{
    // Fee Types
    Task<List<FeeTypeDto>> GetAllFeeTypesAsync();
    Task<FeeTypeDto?> GetFeeTypeByIdAsync(int feeTypeId);
    Task<FeeTypeDto> CreateFeeTypeAsync(CreateFeeTypeDto dto);
    Task UpdateFeeTypeAsync(int feeTypeId, CreateFeeTypeDto dto);
    Task SetFeeTypeActiveAsync(int feeTypeId, bool isActive);

    // Admin Student Fees
    Task<List<StudentFeeDto>> GetAllStudentFeesAsync();
    Task<StudentFeeDto?> GetStudentFeeByIdAsync(int studentFeeId);
    Task<List<StudentFeeDto>> GetStudentFeesByStudentIdAsync(int studentId);
    Task<StudentFeeDto> CreateStudentFeeAsync(CreateStudentFeeDto dto);
    Task UpdateStudentFeeAsync(int studentFeeId, CreateStudentFeeDto dto);

    // Student Self-Service
    Task<List<StudentFeeDto>> GetMyFeesAsync(int userId);
    Task<StudentFeeDto?> GetMyFeeByIdAsync(int userId, int studentFeeId);
    Task<List<FeePaymentDto>> GetMyPaymentsAsync(int userId);
    Task<FeePaymentDto> PayMyFeeAsync(
        int userId,
        int studentFeeId,
        SimulateFeePaymentDto dto);

    // Admin Payments
    Task<List<FeePaymentDto>> GetAllPaymentsAsync();
    Task<FeePaymentDto?> GetPaymentByIdAsync(int feePaymentId);
    Task<List<FeePaymentDto>> GetPaymentsByStudentFeeIdAsync(int studentFeeId);

    // Exam Fee Carry-Forward
    Task<FeePaymentDto> CarryForwardExamFeeAsync(
        int adminUserId,
        CarryForwardFeeDto dto);

    // Refunds
    Task<List<RefundRequestDto>> GetAllRefundRequestsAsync();
    Task<RefundRequestDto?> GetRefundRequestByIdAsync(int refundRequestId);
    Task<List<RefundRequestDto>> GetMyRefundRequestsAsync(int userId);
    Task<RefundRequestDto> CreateMyRefundRequestAsync(
        int userId,
        int paymentId,
        CreateRefundRequestDto dto);
    Task<RefundRequestDto> ReviewRefundRequestAsync(
        int adminUserId,
        int refundRequestId,
        ReviewRefundRequestDto dto);
    Task<RefundRequestDto> ProcessRefundAsync(
        int adminUserId,
        int refundRequestId);
}
