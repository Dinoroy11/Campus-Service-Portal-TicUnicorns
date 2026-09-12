using CampusServicePortal.Modules.Leave.DTOs;

namespace CampusServicePortal.Modules.Leave.Interfaces.Service;

public interface ILeaveService
{
    Task<IReadOnlyCollection<LeaveTypeDto>> GetLeaveTypesAsync(
        bool includeInactive = false);

    Task<LeaveTypeDto> CreateLeaveTypeAsync(CreateLeaveTypeDto dto);

    Task<LeaveTypeDto?> SetLeaveTypeStatusAsync(
        int leaveTypeId,
        bool isActive);

    Task<LeaveDto> CreateLeaveAsync(
        int currentUserId,
        CreateLeaveDto dto);

    Task<IReadOnlyCollection<LeaveDto>> GetMyLeavesAsync(int currentUserId);

    Task<LeaveDto?> GetMyLeaveByIdAsync(
        int currentUserId,
        int leaveRequestId);

    Task<IReadOnlyCollection<LeaveDto>> GetStaffDashboardAsync(
        int currentUserId,
        bool isAdmin);

    Task<LeaveDto?> GetStaffLeaveByIdAsync(
        int currentUserId,
        bool isAdmin,
        int leaveRequestId);

    Task<LeaveDto?> DecideLeaveAsync(
        int currentUserId,
        bool isAdmin,
        int leaveRequestId,
        UpdateLeaveDto dto);
}
