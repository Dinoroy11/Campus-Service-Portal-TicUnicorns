using CampusServicePortal.Modules.Fees.DTOs;

namespace CampusServicePortal.Modules.Fees.Interfaces.Service;

public interface IFeesService
{
    // Fee Types
    Task<List<FeeTypeDto>> GetAllFeeTypesAsync();

    Task<FeeTypeDto?> GetFeeTypeByIdAsync(int feeTypeId);

    Task<FeeTypeDto> CreateFeeTypeAsync(
        CreateFeeTypeDto dto);

    Task UpdateFeeTypeAsync(
        int feeTypeId,
        CreateFeeTypeDto dto);


    // Student Fees
    Task<List<StudentFeeDto>> GetAllStudentFeesAsync();

    Task<StudentFeeDto?> GetStudentFeeByIdAsync(
        int studentFeeId);

    Task<List<StudentFeeDto>> GetStudentFeesByStudentIdAsync(
        int studentId);

    Task<StudentFeeDto> CreateStudentFeeAsync(
        CreateStudentFeeDto dto);

    Task UpdateStudentFeeAsync(
        int studentFeeId,
        CreateStudentFeeDto dto);


    // Fee Payments
    Task<List<FeePaymentDto>> GetAllPaymentsAsync();

    Task<FeePaymentDto?> GetPaymentByIdAsync(
        int feePaymentId);

    Task<List<FeePaymentDto>> GetPaymentsByStudentFeeIdAsync(
        int studentFeeId);

    Task<FeePaymentDto> CreatePaymentAsync(
        FeePaymentDto dto);


    // Refund Requests
    Task<List<RefundRequestDto>> GetAllRefundRequestsAsync();

    Task<RefundRequestDto?> GetRefundRequestByIdAsync(
        int refundRequestId);

    Task<RefundRequestDto?> GetRefundRequestByPaymentIdAsync(
        int paymentId);

    Task<RefundRequestDto> CreateRefundRequestAsync(
        RefundRequestDto dto);

    Task UpdateRefundRequestAsync(
        int refundRequestId,
        RefundRequestDto dto);
}