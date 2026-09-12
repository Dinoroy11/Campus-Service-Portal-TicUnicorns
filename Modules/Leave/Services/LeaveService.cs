using CampusServicePortal.Modules.Identity.Entities;
using CampusServicePortal.Modules.Leave.DTOs;
using CampusServicePortal.Modules.Leave.Entities;
using CampusServicePortal.Modules.Leave.Enums;
using CampusServicePortal.Modules.Leave.Interfaces.Repository;
using CampusServicePortal.Modules.Leave.Interfaces.Service;
using CampusServicePortal.Modules.Notifications.DTOs;
using CampusServicePortal.Modules.Notifications.Interfaces.Service;

namespace CampusServicePortal.Modules.Leave.Services;

public class LeaveService : ILeaveService
{
    private readonly ILeaveRepository _leaveRepository;
    private readonly INotificationService _notificationService;

    public LeaveService(
        ILeaveRepository leaveRepository,
        INotificationService notificationService)
    {
        _leaveRepository = leaveRepository;
        _notificationService = notificationService;
    }

    public async Task<IReadOnlyCollection<LeaveTypeDto>> GetLeaveTypesAsync(
        bool includeInactive = false)
    {
        var items = await _leaveRepository
            .GetLeaveTypesAsync(activeOnly: !includeInactive);

        return items.Select(MapType).ToList();
    }

    public async Task<LeaveTypeDto> CreateLeaveTypeAsync(CreateLeaveTypeDto dto)
    {
        var name = dto.Name?.Trim() ?? string.Empty;

        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Leave type name is required.");

        if (await _leaveRepository.LeaveTypeNameExistsAsync(name))
            throw new InvalidOperationException("Leave type already exists.");

        var leaveType = new LeaveType
        {
            Name = name,
            Description = string.IsNullOrWhiteSpace(dto.Description)
                ? null
                : dto.Description.Trim(),
            IsActive = true
        };

        await _leaveRepository.AddLeaveTypeAsync(leaveType);
        return MapType(leaveType);
    }

    public async Task<LeaveTypeDto?> SetLeaveTypeStatusAsync(
        int leaveTypeId,
        bool isActive)
    {
        var leaveType = await _leaveRepository.GetLeaveTypeByIdAsync(leaveTypeId);
        if (leaveType is null)
            return null;

        leaveType.IsActive = isActive;
        await _leaveRepository.UpdateLeaveTypeAsync(leaveType);

        return MapType(leaveType);
    }

    public async Task<LeaveDto> CreateLeaveAsync(
        int currentUserId,
        CreateLeaveDto dto)
    {
        if (currentUserId <= 0)
            throw new UnauthorizedAccessException("Invalid authenticated user.");

        if (dto.LeaveTypeId <= 0)
            throw new ArgumentException("Leave type is required.");

        var startDate = dto.StartDate.Date;
        var endDate = dto.EndDate.Date;

        if (startDate < DateTime.UtcNow.Date)
            throw new ArgumentException("Leave start date cannot be in the past.");

        if (endDate < startDate)
            throw new ArgumentException("Leave end date cannot be before start date.");

        if (string.IsNullOrWhiteSpace(dto.Reason))
            throw new ArgumentException("Leave reason is required.");

        var leaveType = await _leaveRepository.GetLeaveTypeByIdAsync(dto.LeaveTypeId);
        if (leaveType is null || !leaveType.IsActive)
            throw new InvalidOperationException("Selected leave type is not available.");

        var student = await _leaveRepository.GetStudentByUserIdAsync(currentUserId);
        if (student is null || !student.IsActive)
            throw new UnauthorizedAccessException("Active student profile not found.");

        var masterStudent = await _leaveRepository
            .GetMasterStudentByIdAsync(student.MasterStudentId);

        if (masterStudent is null || !masterStudent.IsActive)
            throw new InvalidOperationException("Student master record is not active.");

        if (masterStudent.DepartmentId <= 0)
            throw new InvalidOperationException("Student department is not configured.");

        var request = new LeaveRequest
        {
            LeaveTypeId = leaveType.LeaveTypeId,
            StudentId = student.StudentId,
            DepartmentId = masterStudent.DepartmentId,
            StartDate = startDate,
            EndDate = endDate,
            Reason = dto.Reason.Trim(),
            Status = LeaveStatus.Pending.ToString(),
            CreatedAt = DateTime.UtcNow
        };

        await _leaveRepository.AddRequestAsync(request);

        var saved = await _leaveRepository.GetRequestByIdAsync(request.LeaveRequestId);
        return MapRequest(saved ?? request);
    }

    public async Task<IReadOnlyCollection<LeaveDto>> GetMyLeavesAsync(
        int currentUserId)
    {
        var student = await RequireStudentAsync(currentUserId);

        var requests = await _leaveRepository
            .GetRequestsByStudentIdAsync(student.StudentId);

        return requests.Select(MapRequest).ToList();
    }

    public async Task<LeaveDto?> GetMyLeaveByIdAsync(
        int currentUserId,
        int leaveRequestId)
    {
        var student = await RequireStudentAsync(currentUserId);
        var request = await _leaveRepository.GetRequestByIdAsync(leaveRequestId);

        if (request is null || request.StudentId != student.StudentId)
            return null;

        return MapRequest(request);
    }

    public async Task<IReadOnlyCollection<LeaveDto>> GetStaffDashboardAsync(
        int currentUserId,
        bool isAdmin)
    {
        IReadOnlyCollection<LeaveRequest> requests;

        if (isAdmin)
        {
            requests = await _leaveRepository.GetAllRequestsAsync();
        }
        else
        {
            var departments = await _leaveRepository
                .GetAssignedDepartmentIdsAsync(currentUserId);

            if (departments.Count == 0)
                throw new UnauthorizedAccessException(
                    "No department staff assignment exists for this user.");

            requests = await _leaveRepository
                .GetRequestsByDepartmentIdsAsync(departments);
        }

        return requests.Select(MapRequest).ToList();
    }

    public async Task<LeaveDto?> GetStaffLeaveByIdAsync(
        int currentUserId,
        bool isAdmin,
        int leaveRequestId)
    {
        var request = await _leaveRepository.GetRequestByIdAsync(leaveRequestId);
        if (request is null)
            return null;

        await EnsureDepartmentScopeAsync(currentUserId, isAdmin, request.DepartmentId);
        return MapRequest(request);
    }

    public async Task<LeaveDto?> DecideLeaveAsync(
        int currentUserId,
        bool isAdmin,
        int leaveRequestId,
        UpdateLeaveDto dto)
    {
        var request = await _leaveRepository.GetRequestByIdAsync(leaveRequestId);
        if (request is null)
            return null;

        await EnsureDepartmentScopeAsync(currentUserId, isAdmin, request.DepartmentId);

        if (!string.Equals(
                request.Status,
                LeaveStatus.Pending.ToString(),
                StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException(
                "Only pending leave requests can be approved or rejected.");
        }

        var newStatus = NormalizeDecision(dto.Status);
        var oldStatus = request.Status;

        request.Status = newStatus;
        await _leaveRepository.UpdateRequestAsync(request);

        var approval = new LeaveApproval
        {
            LeaveRequestId = request.LeaveRequestId,
            ApprovedByUserId = currentUserId,
            Status = newStatus,
            Remarks = string.IsNullOrWhiteSpace(dto.Remarks)
                ? null
                : dto.Remarks.Trim(),
            ApprovedAt = DateTime.UtcNow
        };

        await _leaveRepository.AddApprovalAsync(approval);

        await _leaveRepository.AddAuditLogAsync(new AuditLog
        {
            UserId = currentUserId,
            EntityType = "LeaveRequest",
            EntityId = request.LeaveRequestId,
            Action = "LeaveDecision",
            OldValue = oldStatus,
            NewValue = newStatus,
            CreatedAt = DateTime.UtcNow
        });

        if (request.Student.UserId.HasValue)
        {
            await _notificationService.CreateAsync(new NotificationCreateDto
            {
                UserId = request.Student.UserId.Value,
                Title = $"Leave Request {newStatus}",
                Message = $"Your {request.LeaveType.Name} leave request from " +
                          $"{request.StartDate:yyyy-MM-dd} to {request.EndDate:yyyy-MM-dd} " +
                          $"has been {newStatus.ToLowerInvariant()}.",
                ReferenceType = "LeaveRequest",
                ReferenceId = request.LeaveRequestId
            });
        }

        var updated = await _leaveRepository.GetRequestByIdAsync(leaveRequestId);
        return updated is null ? null : MapRequest(updated);
    }

    private async Task<CampusServicePortal_TicUnicorns.Modules.Students.Entities.Student>
        RequireStudentAsync(int currentUserId)
    {
        var student = await _leaveRepository.GetStudentByUserIdAsync(currentUserId);

        if (student is null || !student.IsActive)
            throw new UnauthorizedAccessException("Active student profile not found.");

        return student;
    }

    private async Task EnsureDepartmentScopeAsync(
        int currentUserId,
        bool isAdmin,
        int departmentId)
    {
        if (isAdmin)
            return;

        var departments = await _leaveRepository
            .GetAssignedDepartmentIdsAsync(currentUserId);

        if (!departments.Contains(departmentId))
            throw new UnauthorizedAccessException(
                "This leave request is outside your assigned department scope.");
    }

    private static string NormalizeDecision(string? status)
    {
        if (string.Equals(
                status,
                LeaveStatus.Approved.ToString(),
                StringComparison.OrdinalIgnoreCase))
            return LeaveStatus.Approved.ToString();

        if (string.Equals(
                status,
                LeaveStatus.Rejected.ToString(),
                StringComparison.OrdinalIgnoreCase))
            return LeaveStatus.Rejected.ToString();

        throw new ArgumentException(
            "Status must be either Approved or Rejected.");
    }

    private static LeaveTypeDto MapType(LeaveType leaveType)
    {
        return new LeaveTypeDto
        {
            LeaveTypeId = leaveType.LeaveTypeId,
            Name = leaveType.Name,
            Description = leaveType.Description,
            IsActive = leaveType.IsActive
        };
    }

    private static LeaveDto MapRequest(LeaveRequest request)
    {
        return new LeaveDto
        {
            LeaveRequestId = request.LeaveRequestId,
            LeaveTypeId = request.LeaveTypeId,
            LeaveTypeName = request.LeaveType?.Name ?? string.Empty,
            StudentId = request.StudentId,
            StudentName = request.Student is null
                ? string.Empty
                : $"{request.Student.FirstName} {request.Student.LastName}".Trim(),
            DepartmentId = request.DepartmentId,
            StartDate = request.StartDate,
            EndDate = request.EndDate,
            Reason = request.Reason,
            Status = request.Status,
            CreatedAt = request.CreatedAt,
            Approvals = request.Approvals?
                .OrderByDescending(x => x.ApprovedAt)
                .Select(x => new LeaveApprovalDto
                {
                    LeaveApprovalId = x.LeaveApprovalId,
                    ApprovedByUserId = x.ApprovedByUserId,
                    Status = x.Status,
                    Remarks = x.Remarks,
                    ApprovedAt = x.ApprovedAt
                })
                .ToList()
                ?? new List<LeaveApprovalDto>()
        };
    }
}
