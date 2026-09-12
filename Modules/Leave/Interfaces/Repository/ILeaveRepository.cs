using CampusServicePortal.Modules.Identity.Entities;
using CampusServicePortal.Modules.Leave.Entities;
using CampusServicePortal_TicUnicorns.Modules.Students.Entities;

namespace CampusServicePortal.Modules.Leave.Interfaces.Repository;

public interface ILeaveRepository
{
    Task<IReadOnlyCollection<LeaveType>> GetLeaveTypesAsync(bool activeOnly);

    Task<LeaveType?> GetLeaveTypeByIdAsync(int leaveTypeId);

    Task<bool> LeaveTypeNameExistsAsync(string name);

    Task AddLeaveTypeAsync(LeaveType leaveType);

    Task UpdateLeaveTypeAsync(LeaveType leaveType);

    Task<Student?> GetStudentByUserIdAsync(int userId);

    Task<StudentMasterList?> GetMasterStudentByIdAsync(int masterStudentId);

    Task<IReadOnlyCollection<LeaveRequest>> GetRequestsByStudentIdAsync(int studentId);

    Task<IReadOnlyCollection<LeaveRequest>> GetRequestsByDepartmentIdsAsync(
        IReadOnlyCollection<int> departmentIds);

    Task<IReadOnlyCollection<LeaveRequest>> GetAllRequestsAsync();

    Task<LeaveRequest?> GetRequestByIdAsync(int leaveRequestId);

    Task AddRequestAsync(LeaveRequest request);

    Task UpdateRequestAsync(LeaveRequest request);

    Task AddApprovalAsync(LeaveApproval approval);

    Task<IReadOnlyCollection<int>> GetAssignedDepartmentIdsAsync(int userId);

    Task AddAuditLogAsync(AuditLog auditLog);
}
