using CampusServicePortal.Modules.Fees.DTOs;
using CampusServicePortal.Modules.Fees.Entities;
using CampusServicePortal.Modules.Fees.Interfaces.Repository;
using CampusServicePortal.Modules.Fees.Interfaces.Service;
using CampusServicePortal.Modules.Notifications.DTOs;
using CampusServicePortal.Modules.Notifications.Interfaces.Service;
using CampusServicePortal_TicUnicorns.Modules.Fees.Enums;
using CampusServicePortal_TicUnicorns.Modules.Students.Entities;
using CampusServicePortal_TicUnicorns.Modules.Students.Interfaces.Repository;

namespace CampusServicePortal.Modules.Fees.Services;

public class FeesService : IFeesService
{
    private readonly IFeeTypeRepository _feeTypeRepository;
    private readonly IStudentFeeRepository _studentFeeRepository;
    private readonly IFeePaymentRepository _feePaymentRepository;
    private readonly IRefundRequestRepository _refundRequestRepository;
    private readonly IStudentRepository _studentRepository;
    private readonly INotificationService _notificationService;

    public FeesService(
        IFeeTypeRepository feeTypeRepository,
        IStudentFeeRepository studentFeeRepository,
        IFeePaymentRepository feePaymentRepository,
        IRefundRequestRepository refundRequestRepository,
        IStudentRepository studentRepository,
        INotificationService notificationService)
    {
        _feeTypeRepository = feeTypeRepository;
        _studentFeeRepository = studentFeeRepository;
        _feePaymentRepository = feePaymentRepository;
        _refundRequestRepository = refundRequestRepository;
        _studentRepository = studentRepository;
        _notificationService = notificationService;
    }

    // =========================================================
    // Fee Types
    // =========================================================

    public async Task<List<FeeTypeDto>> GetAllFeeTypesAsync()
    {
        var items = await _feeTypeRepository.GetAllAsync();
        return items.Select(MapFeeTypeToDto).ToList();
    }

    public async Task<FeeTypeDto?> GetFeeTypeByIdAsync(int feeTypeId)
    {
        var item = await _feeTypeRepository.GetByIdAsync(feeTypeId);
        return item == null ? null : MapFeeTypeToDto(item);
    }

    public async Task<FeeTypeDto> CreateFeeTypeAsync(CreateFeeTypeDto dto)
    {
        ValidateFeeTypeDto(dto);

        var existing = await _feeTypeRepository.GetByNameAsync(dto.Name.Trim());
        if (existing != null)
            throw new InvalidOperationException("A fee type with this name already exists.");

        var entity = new FeeType
        {
            Name = dto.Name.Trim(),
            Description = dto.Description?.Trim() ?? string.Empty,
            Amount = dto.Amount,
            IsActive = true
        };

        await _feeTypeRepository.CreateAsync(entity);
        return MapFeeTypeToDto(entity);
    }

    public async Task UpdateFeeTypeAsync(int feeTypeId, CreateFeeTypeDto dto)
    {
        ValidateFeeTypeDto(dto);

        var entity = await _feeTypeRepository.GetByIdAsync(feeTypeId)
            ?? throw new KeyNotFoundException("Fee type not found.");

        var duplicate = await _feeTypeRepository.GetByNameAsync(dto.Name.Trim());
        if (duplicate != null && duplicate.FeeTypeId != feeTypeId)
            throw new InvalidOperationException("A fee type with this name already exists.");

        entity.Name = dto.Name.Trim();
        entity.Description = dto.Description?.Trim() ?? string.Empty;
        entity.Amount = dto.Amount;

        await _feeTypeRepository.UpdateAsync(entity);
    }

    public async Task SetFeeTypeActiveAsync(int feeTypeId, bool isActive)
    {
        var entity = await _feeTypeRepository.GetByIdAsync(feeTypeId)
            ?? throw new KeyNotFoundException("Fee type not found.");

        entity.IsActive = isActive;
        await _feeTypeRepository.UpdateAsync(entity);
    }

    // =========================================================
    // Admin Student Fees
    // =========================================================

    public async Task<List<StudentFeeDto>> GetAllStudentFeesAsync()
    {
        var items = await _studentFeeRepository.GetAllAsync();
        return items.Select(MapStudentFeeToDto).ToList();
    }

    public async Task<StudentFeeDto?> GetStudentFeeByIdAsync(int studentFeeId)
    {
        var item = await _studentFeeRepository.GetByIdAsync(studentFeeId);
        return item == null ? null : MapStudentFeeToDto(item);
    }

    public async Task<List<StudentFeeDto>> GetStudentFeesByStudentIdAsync(int studentId)
    {
        var student = await _studentRepository.GetByIdAsync(studentId);
        if (student == null)
            throw new KeyNotFoundException("Student not found.");

        var items = await _studentFeeRepository.GetByStudentIdAsync(studentId);
        return items.Select(MapStudentFeeToDto).ToList();
    }

    public async Task<StudentFeeDto> CreateStudentFeeAsync(CreateStudentFeeDto dto)
    {
        ValidateStudentFeeDto(dto);

        var student = await _studentRepository.GetByIdAsync(dto.StudentId)
            ?? throw new KeyNotFoundException("Student not found.");

        if (!student.IsActive)
            throw new InvalidOperationException("Cannot assign a fee to an inactive student.");

        var feeType = await _feeTypeRepository.GetByIdAsync(dto.FeeTypeId)
            ?? throw new KeyNotFoundException("Fee type not found.");

        if (!feeType.IsActive)
            throw new InvalidOperationException("This fee type is inactive.");

        if (!string.IsNullOrWhiteSpace(dto.ExamReference))
        {
            var existing = await _studentFeeRepository.GetByReferenceAsync(
                dto.StudentId,
                dto.ExamReference.Trim());

            if (existing != null)
                throw new InvalidOperationException(
                    "A fee already exists for this student and exam reference.");
        }

        var entity = new StudentFee
        {
            StudentId = dto.StudentId,
            FeeTypeId = dto.FeeTypeId,
            Amount = dto.Amount,
            DueDate = dto.DueDate,
            Status = ResolveOutstandingStatus(dto.DueDate),
            ExamReference = Normalize(dto.ExamReference),
            CreatedAt = DateTime.UtcNow
        };

        await _studentFeeRepository.CreateAsync(entity);

        await NotifyStudentAsync(
            student,
            "New Fee Assigned",
            $"A fee of LKR {entity.Amount:0.00} has been assigned to your account.",
            "StudentFee",
            entity.StudentFeeId);

        return MapStudentFeeToDto(entity);
    }

    public async Task UpdateStudentFeeAsync(int studentFeeId, CreateStudentFeeDto dto)
    {
        ValidateStudentFeeDto(dto);

        var entity = await _studentFeeRepository.GetByIdAsync(studentFeeId)
            ?? throw new KeyNotFoundException("Student fee not found.");

        if (entity.Status is StudentFeeStatus.Paid or StudentFeeStatus.Cancelled)
            throw new InvalidOperationException("Paid or cancelled fees cannot be edited.");

        if (dto.StudentId != entity.StudentId)
            throw new InvalidOperationException("A fee cannot be moved to another student.");

        var feeType = await _feeTypeRepository.GetByIdAsync(dto.FeeTypeId)
            ?? throw new KeyNotFoundException("Fee type not found.");

        if (!feeType.IsActive)
            throw new InvalidOperationException("This fee type is inactive.");

        entity.FeeTypeId = dto.FeeTypeId;
        entity.Amount = dto.Amount;
        entity.DueDate = dto.DueDate;
        entity.ExamReference = Normalize(dto.ExamReference);
        entity.Status = ResolveOutstandingStatus(dto.DueDate);

        await _studentFeeRepository.UpdateAsync(entity);
    }

    // =========================================================
    // Student Self-Service
    // =========================================================

    public async Task<List<StudentFeeDto>> GetMyFeesAsync(int userId)
    {
        var student = await GetStudentByUserIdAsync(userId);
        var items = await _studentFeeRepository.GetByStudentIdAsync(student.StudentId);
        return items.Select(MapStudentFeeToDto).ToList();
    }

    public async Task<StudentFeeDto?> GetMyFeeByIdAsync(int userId, int studentFeeId)
    {
        var student = await GetStudentByUserIdAsync(userId);
        var item = await _studentFeeRepository.GetByIdAsync(studentFeeId);

        if (item == null || item.StudentId != student.StudentId)
            return null;

        return MapStudentFeeToDto(item);
    }

    public async Task<List<FeePaymentDto>> GetMyPaymentsAsync(int userId)
    {
        var student = await GetStudentByUserIdAsync(userId);
        var fees = await _studentFeeRepository.GetByStudentIdAsync(student.StudentId);

        var result = new List<FeePaymentDto>();
        foreach (var fee in fees)
        {
            var payments = await _feePaymentRepository.GetByStudentFeeIdAsync(fee.StudentFeeId);
            result.AddRange(payments.Select(MapPaymentToDto));
        }

        return result
            .OrderByDescending(x => x.FeePaymentId)
            .ToList();
    }

    public async Task<FeePaymentDto> PayMyFeeAsync(
        int userId,
        int studentFeeId,
        SimulateFeePaymentDto dto)
    {
        var student = await GetStudentByUserIdAsync(userId);
        var fee = await _studentFeeRepository.GetByIdAsync(studentFeeId)
            ?? throw new KeyNotFoundException("Student fee not found.");

        EnsureStudentOwnsFee(student, fee);

        if (fee.Status == StudentFeeStatus.Cancelled)
            throw new InvalidOperationException("Cancelled fees cannot be paid.");

        if (fee.Status == StudentFeeStatus.Paid ||
            await _feePaymentRepository.HasPaidPaymentForStudentFeeAsync(studentFeeId))
        {
            throw new InvalidOperationException("This fee has already been paid.");
        }

        if (fee.Amount <= 0)
            throw new InvalidOperationException("The fee amount is invalid.");

        var payment = new FeePayment
        {
            StudentFeeId = fee.StudentFeeId,
            Amount = fee.Amount,
            PaymentStatus = PaymentStatus.Paid,
            PaymentReference = string.IsNullOrWhiteSpace(dto?.PaymentReference)
                ? $"SIM-FEE-{fee.StudentFeeId}-{DateTime.UtcNow:yyyyMMddHHmmss}"
                : dto.PaymentReference.Trim(),
            PaidAt = DateTime.UtcNow,
            SourcePaymentId = null,
            SourceFeeId = null,
            TargetStudentFeeId = null
        };

        await _feePaymentRepository.CreateAsync(payment);

        fee.Status = StudentFeeStatus.Paid;
        await _studentFeeRepository.UpdateAsync(fee);

        await NotifyStudentAsync(
            student,
            "Fee Payment Successful",
            $"Your simulated fee payment of LKR {payment.Amount:0.00} was completed successfully.",
            "FeePayment",
            payment.FeePaymentId);

        return MapPaymentToDto(payment);
    }

    // =========================================================
    // Admin Payments
    // =========================================================

    public async Task<List<FeePaymentDto>> GetAllPaymentsAsync()
    {
        var items = await _feePaymentRepository.GetAllAsync();
        return items.Select(MapPaymentToDto).ToList();
    }

    public async Task<FeePaymentDto?> GetPaymentByIdAsync(int feePaymentId)
    {
        var item = await _feePaymentRepository.GetByIdAsync(feePaymentId);
        return item == null ? null : MapPaymentToDto(item);
    }

    public async Task<List<FeePaymentDto>> GetPaymentsByStudentFeeIdAsync(int studentFeeId)
    {
        var fee = await _studentFeeRepository.GetByIdAsync(studentFeeId)
            ?? throw new KeyNotFoundException("Student fee not found.");

        var items = await _feePaymentRepository.GetByStudentFeeIdAsync(fee.StudentFeeId);
        return items.Select(MapPaymentToDto).ToList();
    }

    // =========================================================
    // Exam Fee Carry-Forward
    // =========================================================

    public async Task<FeePaymentDto> CarryForwardExamFeeAsync(
        int adminUserId,
        CarryForwardFeeDto dto)
    {
        if (adminUserId <= 0)
            throw new UnauthorizedAccessException("Invalid admin identity.");

        if (dto.SourcePaymentId <= 0 || dto.TargetStudentFeeId <= 0)
            throw new ArgumentException("Source payment and target fee are required.");

        var sourcePayment = await _feePaymentRepository.GetByIdAsync(dto.SourcePaymentId)
            ?? throw new KeyNotFoundException("Source payment not found.");

        if (sourcePayment.PaymentStatus != PaymentStatus.Paid)
            throw new InvalidOperationException("Only a paid payment can be carried forward.");

        if (await _feePaymentRepository.HasCarryForwardFromPaymentAsync(sourcePayment.FeePaymentId))
            throw new InvalidOperationException("This payment has already been carried forward.");

        var existingRefund = await _refundRequestRepository.GetByPaymentIdAsync(sourcePayment.FeePaymentId);
        if (existingRefund != null && existingRefund.Status != RefundStatus.Rejected &&
            existingRefund.Status != RefundStatus.Cancelled)
        {
            throw new InvalidOperationException(
                "This payment has an active refund request and cannot be carried forward.");
        }

        var sourceFee = await _studentFeeRepository.GetByIdAsync(sourcePayment.StudentFeeId)
            ?? throw new KeyNotFoundException("Source student fee not found.");

        var targetFee = await _studentFeeRepository.GetByIdAsync(dto.TargetStudentFeeId)
            ?? throw new KeyNotFoundException("Target student fee not found.");

        if (sourceFee.StudentId != targetFee.StudentId)
            throw new InvalidOperationException("Carry-forward must stay with the same student.");

        if (string.IsNullOrWhiteSpace(sourceFee.ExamReference) ||
            string.IsNullOrWhiteSpace(targetFee.ExamReference))
        {
            throw new InvalidOperationException(
                "Carry-forward is supported only for examination fees with exam references.");
        }

        if (sourceFee.StudentFeeId == targetFee.StudentFeeId)
            throw new InvalidOperationException("Source and target fee cannot be the same.");

        if (targetFee.Status is StudentFeeStatus.Paid or StudentFeeStatus.Cancelled ||
            await _feePaymentRepository.HasPaidPaymentForStudentFeeAsync(targetFee.StudentFeeId))
        {
            throw new InvalidOperationException("Target fee is already settled or cancelled.");
        }

        if (decimal.Round(sourcePayment.Amount, 2) != decimal.Round(targetFee.Amount, 2))
        {
            throw new InvalidOperationException(
                "For this project flow, the source payment and target exam fee amounts must match.");
        }

        var transferPayment = new FeePayment
        {
            StudentFeeId = targetFee.StudentFeeId,
            Amount = sourcePayment.Amount,
            PaymentStatus = PaymentStatus.Paid,
            PaymentReference = $"CF-{sourcePayment.FeePaymentId}-{targetFee.StudentFeeId}",
            PaidAt = DateTime.UtcNow,
            SourcePaymentId = sourcePayment.FeePaymentId,
            SourceFeeId = sourceFee.StudentFeeId,
            TargetStudentFeeId = targetFee.StudentFeeId
        };

        await _feePaymentRepository.CreateAsync(transferPayment);

        sourceFee.Status = StudentFeeStatus.Cancelled;
        targetFee.Status = StudentFeeStatus.Paid;

        await _studentFeeRepository.UpdateAsync(sourceFee);
        await _studentFeeRepository.UpdateAsync(targetFee);

        var student = await _studentRepository.GetByIdAsync(sourceFee.StudentId);
        if (student != null)
        {
            await NotifyStudentAsync(
                student,
                "Exam Fee Carried Forward",
                $"Your payment for '{sourceFee.ExamReference}' was carried forward to '{targetFee.ExamReference}'.",
                "FeePayment",
                transferPayment.FeePaymentId);
        }

        return MapPaymentToDto(transferPayment);
    }

    // =========================================================
    // Refunds
    // =========================================================

    public async Task<List<RefundRequestDto>> GetAllRefundRequestsAsync()
    {
        var items = await _refundRequestRepository.GetAllAsync();
        return items.Select(MapRefundToDto).ToList();
    }

    public async Task<RefundRequestDto?> GetRefundRequestByIdAsync(int refundRequestId)
    {
        var item = await _refundRequestRepository.GetByIdAsync(refundRequestId);
        return item == null ? null : MapRefundToDto(item);
    }

    public async Task<List<RefundRequestDto>> GetMyRefundRequestsAsync(int userId)
    {
        var student = await GetStudentByUserIdAsync(userId);
        var fees = await _studentFeeRepository.GetByStudentIdAsync(student.StudentId);
        var feeIds = fees.Select(x => x.StudentFeeId).ToHashSet();

        var payments = await _feePaymentRepository.GetAllAsync();
        var paymentIds = payments
            .Where(x => feeIds.Contains(x.StudentFeeId))
            .Select(x => x.FeePaymentId)
            .ToHashSet();

        var refunds = await _refundRequestRepository.GetAllAsync();
        return refunds
            .Where(x => paymentIds.Contains(x.PaymentId))
            .Select(MapRefundToDto)
            .ToList();
    }

    public async Task<RefundRequestDto> CreateMyRefundRequestAsync(
        int userId,
        int paymentId,
        CreateRefundRequestDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto?.Reason))
            throw new ArgumentException("Refund reason is required.");

        if (dto.Reason.Trim().Length > 1000)
            throw new ArgumentException("Refund reason cannot exceed 1000 characters.");

        var student = await GetStudentByUserIdAsync(userId);
        var payment = await _feePaymentRepository.GetByIdAsync(paymentId)
            ?? throw new KeyNotFoundException("Payment not found.");

        var fee = await _studentFeeRepository.GetByIdAsync(payment.StudentFeeId)
            ?? throw new KeyNotFoundException("Student fee not found.");

        EnsureStudentOwnsFee(student, fee);

        if (payment.PaymentStatus != PaymentStatus.Paid)
            throw new InvalidOperationException("Only a paid payment can be refunded.");

        if (string.IsNullOrWhiteSpace(fee.ExamReference))
        {
            throw new InvalidOperationException(
                "Refund requests in this flow are supported only for examination fee payments.");
        }

        if (await _feePaymentRepository.HasCarryForwardFromPaymentAsync(payment.FeePaymentId))
            throw new InvalidOperationException("A carried-forward payment cannot also be refunded.");

        if (await _refundRequestRepository.ExistsForPaymentAsync(payment.FeePaymentId))
            throw new InvalidOperationException("A refund request already exists for this payment.");

        var refund = new RefundRequest
        {
            PaymentId = payment.FeePaymentId,
            Reason = dto.Reason.Trim(),
            Amount = payment.Amount,
            Status = RefundStatus.Pending,
            RequestedAt = DateTime.UtcNow,
            ReviewedByUserId = null,
            ProcessedAt = null
        };

        await _refundRequestRepository.CreateAsync(refund);
        return MapRefundToDto(refund);
    }

    public async Task<RefundRequestDto> ReviewRefundRequestAsync(
        int adminUserId,
        int refundRequestId,
        ReviewRefundRequestDto dto)
    {
        if (adminUserId <= 0)
            throw new UnauthorizedAccessException("Invalid admin identity.");

        var refund = await _refundRequestRepository.GetByIdAsync(refundRequestId)
            ?? throw new KeyNotFoundException("Refund request not found.");

        if (refund.Status != RefundStatus.Pending)
            throw new InvalidOperationException("Only pending refund requests can be reviewed.");

        if (!Enum.TryParse<RefundStatus>(dto?.Status, true, out var newStatus) ||
            (newStatus != RefundStatus.Approved && newStatus != RefundStatus.Rejected))
        {
            throw new ArgumentException("Status must be Approved or Rejected.");
        }

        refund.Status = newStatus;
        refund.ReviewedByUserId = adminUserId;
        refund.ProcessedAt = newStatus == RefundStatus.Rejected
            ? DateTime.UtcNow
            : null;

        await _refundRequestRepository.UpdateAsync(refund);

        await NotifyRefundOwnerAsync(
            refund,
            "Refund Request Updated",
            newStatus == RefundStatus.Approved
                ? "Your refund request has been approved and is waiting for simulated processing."
                : "Your refund request has been rejected.");

        return MapRefundToDto(refund);
    }

    public async Task<RefundRequestDto> ProcessRefundAsync(
        int adminUserId,
        int refundRequestId)
    {
        if (adminUserId <= 0)
            throw new UnauthorizedAccessException("Invalid admin identity.");

        var refund = await _refundRequestRepository.GetByIdAsync(refundRequestId)
            ?? throw new KeyNotFoundException("Refund request not found.");

        if (refund.Status != RefundStatus.Approved)
            throw new InvalidOperationException("Only an approved refund can be processed.");

        var payment = await _feePaymentRepository.GetByIdAsync(refund.PaymentId)
            ?? throw new KeyNotFoundException("Payment not found.");

        if (payment.PaymentStatus != PaymentStatus.Paid)
            throw new InvalidOperationException("The payment is no longer refundable.");

        var fee = await _studentFeeRepository.GetByIdAsync(payment.StudentFeeId)
            ?? throw new KeyNotFoundException("Student fee not found.");

        payment.PaymentStatus = PaymentStatus.Refunded;
        refund.Status = RefundStatus.Processed;
        refund.ReviewedByUserId = adminUserId;
        refund.ProcessedAt = DateTime.UtcNow;
        fee.Status = StudentFeeStatus.Cancelled;

        await _feePaymentRepository.UpdateAsync(payment);
        await _studentFeeRepository.UpdateAsync(fee);
        await _refundRequestRepository.UpdateAsync(refund);

        await NotifyRefundOwnerAsync(
            refund,
            "Refund Processed",
            $"Your simulated refund of LKR {refund.Amount:0.00} has been processed.");

        return MapRefundToDto(refund);
    }

    // =========================================================
    // Helpers
    // =========================================================

    private async Task<Student> GetStudentByUserIdAsync(int userId)
    {
        if (userId <= 0)
            throw new UnauthorizedAccessException("Invalid user identity.");

        var student = await _studentRepository.GetByUserIdAsync(userId);
        if (student == null)
            throw new UnauthorizedAccessException("Student profile not found for this user.");

        if (!student.IsActive)
            throw new UnauthorizedAccessException("Student profile is inactive.");

        return student;
    }

    private static void EnsureStudentOwnsFee(Student student, StudentFee fee)
    {
        if (fee.StudentId != student.StudentId)
            throw new UnauthorizedAccessException("You cannot access another student's fee.");
    }

    private async Task NotifyRefundOwnerAsync(
        RefundRequest refund,
        string title,
        string message)
    {
        var payment = await _feePaymentRepository.GetByIdAsync(refund.PaymentId);
        if (payment == null)
            return;

        var fee = await _studentFeeRepository.GetByIdAsync(payment.StudentFeeId);
        if (fee == null)
            return;

        var student = await _studentRepository.GetByIdAsync(fee.StudentId);
        if (student == null)
            return;

        await NotifyStudentAsync(student, title, message, "RefundRequest", refund.RefundRequestId);
    }

    private async Task NotifyStudentAsync(
        Student student,
        string title,
        string message,
        string referenceType,
        int referenceId)
    {
        if (student.UserId is not int userId || userId <= 0)
            return;

        await _notificationService.CreateAsync(new NotificationCreateDto
        {
            UserId = userId,
            Title = title,
            Message = message,
            ReferenceType = referenceType,
            ReferenceId = referenceId
        });
    }

    private static StudentFeeStatus ResolveOutstandingStatus(DateTime? dueDate)
    {
        return dueDate.HasValue && dueDate.Value.Date < DateTime.UtcNow.Date
            ? StudentFeeStatus.Overdue
            : StudentFeeStatus.Outstanding;
    }

    private static void ValidateFeeTypeDto(CreateFeeTypeDto dto)
    {
        if (dto == null)
            throw new ArgumentNullException(nameof(dto));

        if (string.IsNullOrWhiteSpace(dto.Name))
            throw new ArgumentException("Fee type name is required.");

        if (dto.Name.Trim().Length > 100)
            throw new ArgumentException("Fee type name cannot exceed 100 characters.");

        if ((dto.Description?.Trim().Length ?? 0) > 500)
            throw new ArgumentException("Description cannot exceed 500 characters.");

        if (dto.Amount <= 0)
            throw new ArgumentException("Fee type amount must be greater than zero.");
    }

    private static void ValidateStudentFeeDto(CreateStudentFeeDto dto)
    {
        if (dto == null)
            throw new ArgumentNullException(nameof(dto));

        if (dto.StudentId <= 0)
            throw new ArgumentException("A valid student id is required.");

        if (dto.FeeTypeId <= 0)
            throw new ArgumentException("A valid fee type id is required.");

        if (dto.Amount <= 0)
            throw new ArgumentException("Fee amount must be greater than zero.");

        if ((dto.ExamReference?.Trim().Length ?? 0) > 200)
            throw new ArgumentException("Exam reference cannot exceed 200 characters.");
    }

    private static string? Normalize(string? value)
    {
        return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    }

    private static FeeTypeDto MapFeeTypeToDto(FeeType x)
    {
        return new FeeTypeDto
        {
            FeeTypeId = x.FeeTypeId,
            Name = x.Name,
            Description = x.Description,
            Amount = x.Amount,
            IsActive = x.IsActive
        };
    }

    private static StudentFeeDto MapStudentFeeToDto(StudentFee x)
    {
        var status = x.Status;
        if (status == StudentFeeStatus.Outstanding &&
            x.DueDate.HasValue &&
            x.DueDate.Value.Date < DateTime.UtcNow.Date)
        {
            status = StudentFeeStatus.Overdue;
        }

        return new StudentFeeDto
        {
            StudentFeeId = x.StudentFeeId,
            StudentId = x.StudentId,
            FeeTypeId = x.FeeTypeId,
            Amount = x.Amount,
            DueDate = x.DueDate,
            Status = status.ToString(),
            ExamReference = x.ExamReference,
            CreatedAt = x.CreatedAt
        };
    }

    private static FeePaymentDto MapPaymentToDto(FeePayment x)
    {
        return new FeePaymentDto
        {
            FeePaymentId = x.FeePaymentId,
            StudentFeeId = x.StudentFeeId,
            Amount = x.Amount,
            PaymentStatus = x.PaymentStatus.ToString(),
            PaymentReference = x.PaymentReference,
            PaidAt = x.PaidAt,
            SourcePaymentId = x.SourcePaymentId,
            SourceFeeId = x.SourceFeeId,
            TargetStudentFeeId = x.TargetStudentFeeId
        };
    }

    private static RefundRequestDto MapRefundToDto(RefundRequest x)
    {
        return new RefundRequestDto
        {
            RefundRequestId = x.RefundRequestId,
            PaymentId = x.PaymentId,
            Reason = x.Reason,
            Amount = x.Amount,
            Status = x.Status.ToString(),
            RequestedAt = x.RequestedAt,
            ReviewedByUserId = x.ReviewedByUserId,
            ProcessedAt = x.ProcessedAt
        };
    }
}
