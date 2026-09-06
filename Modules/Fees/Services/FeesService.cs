using CampusServicePortal.Modules.Fees.DTOs;
using CampusServicePortal.Modules.Fees.Entities;
using CampusServicePortal.Modules.Fees.Interfaces.Repository;
using CampusServicePortal.Modules.Fees.Interfaces.Service;

namespace CampusServicePortal.Modules.Fees.Services;

public class FeesService : IFeesService
{
    private readonly IFeeTypeRepository _feeTypeRepository;
    private readonly IStudentFeeRepository _studentFeeRepository;
    private readonly IFeePaymentRepository _feePaymentRepository;
    private readonly IRefundRequestRepository _refundRequestRepository;

    public FeesService(
        IFeeTypeRepository feeTypeRepository,
        IStudentFeeRepository studentFeeRepository,
        IFeePaymentRepository feePaymentRepository,
        IRefundRequestRepository refundRequestRepository)
    {
        _feeTypeRepository = feeTypeRepository;
        _studentFeeRepository = studentFeeRepository;
        _feePaymentRepository = feePaymentRepository;
        _refundRequestRepository = refundRequestRepository;
    }

    // =========================================================
    // Fee Types
    // =========================================================

    public async Task<List<FeeTypeDto>> GetAllFeeTypesAsync()
    {
        var feeTypes = await _feeTypeRepository.GetAllAsync();

        return feeTypes.Select(MapFeeTypeToDto).ToList();
    }

    public async Task<FeeTypeDto?> GetFeeTypeByIdAsync(int feeTypeId)
    {
        var feeType = await _feeTypeRepository.GetByIdAsync(feeTypeId);

        if (feeType == null)
            return null;

        return MapFeeTypeToDto(feeType);
    }

    public async Task<FeeTypeDto> CreateFeeTypeAsync(
        CreateFeeTypeDto dto)
    {
        var feeType = new FeeType
        {
            Name = dto.Name,
            Description = dto.Description,
            Amount = dto.Amount,
            IsActive = true
        };

        await _feeTypeRepository.CreateAsync(feeType);

        return MapFeeTypeToDto(feeType);
    }

    public async Task UpdateFeeTypeAsync(
        int feeTypeId,
        CreateFeeTypeDto dto)
    {
        var feeType =
            await _feeTypeRepository.GetByIdAsync(feeTypeId);

        if (feeType == null)
            throw new KeyNotFoundException(
                "Fee type not found.");

        feeType.Name = dto.Name;
        feeType.Description = dto.Description;
        feeType.Amount = dto.Amount;

        await _feeTypeRepository.UpdateAsync(feeType);
    }


    // =========================================================
    // Student Fees
    // =========================================================

    public async Task<List<StudentFeeDto>>
        GetAllStudentFeesAsync()
    {
        var studentFees =
            await _studentFeeRepository.GetAllAsync();

        return studentFees
            .Select(MapStudentFeeToDto)
            .ToList();
    }

    public async Task<StudentFeeDto?>
        GetStudentFeeByIdAsync(int studentFeeId)
    {
        var studentFee =
            await _studentFeeRepository.GetByIdAsync(studentFeeId);

        if (studentFee == null)
            return null;

        return MapStudentFeeToDto(studentFee);
    }

    public async Task<List<StudentFeeDto>>
        GetStudentFeesByStudentIdAsync(int studentId)
    {
        var studentFees =
            await _studentFeeRepository
                .GetByStudentIdAsync(studentId);

        return studentFees
            .Select(MapStudentFeeToDto)
            .ToList();
    }

    public async Task<StudentFeeDto>
        CreateStudentFeeAsync(CreateStudentFeeDto dto)
    {
        var feeTypeExists =
            await _feeTypeRepository.ExistsAsync(dto.FeeTypeId);

        if (!feeTypeExists)
            throw new KeyNotFoundException(
                "Fee type not found.");

        var studentFee = new StudentFee
        {
            StudentId = dto.StudentId,
            FeeTypeId = dto.FeeTypeId,
            Amount = dto.Amount,
            DueDate = dto.DueDate,
            Status = "Outstanding",
            ExamReference = dto.ExamReference,
            CreatedAt = DateTime.UtcNow
        };

        await _studentFeeRepository.CreateAsync(studentFee);

        return MapStudentFeeToDto(studentFee);
    }

    public async Task UpdateStudentFeeAsync(
        int studentFeeId,
        CreateStudentFeeDto dto)
    {
        var studentFee =
            await _studentFeeRepository
                .GetByIdAsync(studentFeeId);

        if (studentFee == null)
            throw new KeyNotFoundException(
                "Student fee not found.");

        // Paid fee cannot be edited.
        if (studentFee.Status.Equals(
                "Paid",
                StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException(
                "Paid fee cannot be edited.");
        }

        var feeTypeExists =
            await _feeTypeRepository.ExistsAsync(dto.FeeTypeId);

        if (!feeTypeExists)
            throw new KeyNotFoundException(
                "Fee type not found.");

        studentFee.FeeTypeId = dto.FeeTypeId;
        studentFee.Amount = dto.Amount;
        studentFee.DueDate = dto.DueDate;
        studentFee.ExamReference = dto.ExamReference;

        await _studentFeeRepository.UpdateAsync(studentFee);
    }


    // =========================================================
    // Fee Payments
    // =========================================================

    public async Task<List<FeePaymentDto>>
        GetAllPaymentsAsync()
    {
        var payments =
            await _feePaymentRepository.GetAllAsync();

        return payments
            .Select(MapPaymentToDto)
            .ToList();
    }

    public async Task<FeePaymentDto?>
        GetPaymentByIdAsync(int feePaymentId)
    {
        var payment =
            await _feePaymentRepository
                .GetByIdAsync(feePaymentId);

        if (payment == null)
            return null;

        return MapPaymentToDto(payment);
    }

    public async Task<List<FeePaymentDto>>
        GetPaymentsByStudentFeeIdAsync(int studentFeeId)
    {
        var payments =
            await _feePaymentRepository
                .GetByStudentFeeIdAsync(studentFeeId);

        return payments
            .Select(MapPaymentToDto)
            .ToList();
    }

    public async Task<FeePaymentDto>
        CreatePaymentAsync(FeePaymentDto dto)
    {
        var studentFee =
            await _studentFeeRepository
                .GetByIdAsync(dto.StudentFeeId);

        if (studentFee == null)
            throw new KeyNotFoundException(
                "Student fee not found.");

        // Prevent duplicate payment.
        var alreadyPaid =
            await _feePaymentRepository
                .ExistsForStudentFeeAsync(dto.StudentFeeId);

        if (alreadyPaid)
            throw new InvalidOperationException(
                "This fee has already been paid.");

        var payment = new FeePayment
        {
            StudentFeeId = dto.StudentFeeId,
            Amount = dto.Amount,
            PaymentStatus = dto.PaymentStatus,
            PaymentReference = dto.PaymentReference,
            PaidAt = dto.PaidAt ?? DateTime.UtcNow,
            SourcePaymentId = dto.SourcePaymentId,
            SourceFeeId = dto.SourceFeeId,
            TargetStudentFeeId = dto.TargetStudentFeeId
        };

        await _feePaymentRepository.CreateAsync(payment);

        return MapPaymentToDto(payment);
    }


    // =========================================================
    // Refund Requests
    // =========================================================

    public async Task<List<RefundRequestDto>>
        GetAllRefundRequestsAsync()
    {
        var refunds =
            await _refundRequestRepository.GetAllAsync();

        return refunds
            .Select(MapRefundToDto)
            .ToList();
    }

    public async Task<RefundRequestDto?>
        GetRefundRequestByIdAsync(int refundRequestId)
    {
        var refund =
            await _refundRequestRepository
                .GetByIdAsync(refundRequestId);

        if (refund == null)
            return null;

        return MapRefundToDto(refund);
    }

    public async Task<RefundRequestDto?>
        GetRefundRequestByPaymentIdAsync(int paymentId)
    {
        var refund =
            await _refundRequestRepository
                .GetByPaymentIdAsync(paymentId);

        if (refund == null)
            return null;

        return MapRefundToDto(refund);
    }

    public async Task<RefundRequestDto>
        CreateRefundRequestAsync(RefundRequestDto dto)
    {
        var payment =
            await _feePaymentRepository
                .GetByIdAsync(dto.PaymentId);

        if (payment == null)
            throw new KeyNotFoundException(
                "Payment not found.");

        var existingRefund =
            await _refundRequestRepository
                .ExistsForPaymentAsync(dto.PaymentId);

        if (existingRefund)
            throw new InvalidOperationException(
                "A refund request already exists for this payment.");

        var refund = new RefundRequest
        {
            PaymentId = dto.PaymentId,
            Reason = dto.Reason,
            Amount = dto.Amount,
            Status = dto.Status,
            RequestedAt = dto.RequestedAt == default
                ? DateTime.UtcNow
                : dto.RequestedAt,
            ReviewedByUserId = dto.ReviewedByUserId,
            ProcessedAt = dto.ProcessedAt
        };

        await _refundRequestRepository
            .CreateAsync(refund);

        return MapRefundToDto(refund);
    }

    public async Task UpdateRefundRequestAsync(
        int refundRequestId,
        RefundRequestDto dto)
    {
        var refund =
            await _refundRequestRepository
                .GetByIdAsync(refundRequestId);

        if (refund == null)
            throw new KeyNotFoundException(
                "Refund request not found.");

        refund.Reason = dto.Reason;
        refund.Amount = dto.Amount;
        refund.Status = dto.Status;
        refund.ReviewedByUserId = dto.ReviewedByUserId;
        refund.ProcessedAt = dto.ProcessedAt;

        await _refundRequestRepository
            .UpdateAsync(refund);
    }


    // =========================================================
    // Mapping
    // =========================================================

    private static FeeTypeDto MapFeeTypeToDto(
        FeeType feeType)
    {
        return new FeeTypeDto
        {
            FeeTypeId = feeType.FeeTypeId,
            Name = feeType.Name,
            Description = feeType.Description,
            Amount = feeType.Amount,
            IsActive = feeType.IsActive
        };
    }

    private static StudentFeeDto MapStudentFeeToDto(
        StudentFee studentFee)
    {
        return new StudentFeeDto
        {
            StudentFeeId = studentFee.StudentFeeId,
            StudentId = studentFee.StudentId,
            FeeTypeId = studentFee.FeeTypeId,
            Amount = studentFee.Amount,
            DueDate = studentFee.DueDate,
            Status = studentFee.Status,
            ExamReference = studentFee.ExamReference,
            CreatedAt = studentFee.CreatedAt
        };
    }

    private static FeePaymentDto MapPaymentToDto(
        FeePayment payment)
    {
        return new FeePaymentDto
        {
            FeePaymentId = payment.FeePaymentId,
            StudentFeeId = payment.StudentFeeId,
            Amount = payment.Amount,
            PaymentStatus = payment.PaymentStatus,
            PaymentReference = payment.PaymentReference,
            PaidAt = payment.PaidAt,
            SourcePaymentId = payment.SourcePaymentId,
            SourceFeeId = payment.SourceFeeId,
            TargetStudentFeeId = payment.TargetStudentFeeId
        };
    }

    private static RefundRequestDto MapRefundToDto(
        RefundRequest refund)
    {
        return new RefundRequestDto
        {
            RefundRequestId = refund.RefundRequestId,
            PaymentId = refund.PaymentId,
            Reason = refund.Reason,
            Amount = refund.Amount,
            Status = refund.Status,
            RequestedAt = refund.RequestedAt,
            ReviewedByUserId = refund.ReviewedByUserId,
            ProcessedAt = refund.ProcessedAt
        };
    }
}