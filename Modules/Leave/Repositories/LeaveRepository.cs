using CampusServicePortal.Modules.Identity.Entities;
using CampusServicePortal.Modules.Leave.Entities;
using CampusServicePortal.Modules.Leave.Interfaces.Repository;
using CampusServicePortal_TicUnicorns.Data;
using CampusServicePortal_TicUnicorns.Modules.Students.Entities;
using Microsoft.EntityFrameworkCore;

namespace CampusServicePortal.Modules.Leave.Repositories;

public class LeaveRepository : ILeaveRepository
{
    private readonly CampusDbContext _context;

    public LeaveRepository(CampusDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyCollection<LeaveType>> GetLeaveTypesAsync(
        bool activeOnly)
    {
        var query = _context.LeaveTypes.AsNoTracking().AsQueryable();

        if (activeOnly)
            query = query.Where(x => x.IsActive);

        return await query
            .OrderBy(x => x.Name)
            .ToListAsync();
    }

    public async Task<LeaveType?> GetLeaveTypeByIdAsync(int leaveTypeId)
    {
        return await _context.LeaveTypes
            .FirstOrDefaultAsync(x => x.LeaveTypeId == leaveTypeId);
    }

    public async Task<bool> LeaveTypeNameExistsAsync(string name)
    {
        var normalized = name.Trim().ToLower();

        return await _context.LeaveTypes
            .AnyAsync(x => x.Name.ToLower() == normalized);
    }

    public async Task AddLeaveTypeAsync(LeaveType leaveType)
    {
        await _context.LeaveTypes.AddAsync(leaveType);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateLeaveTypeAsync(LeaveType leaveType)
    {
        _context.LeaveTypes.Update(leaveType);
        await _context.SaveChangesAsync();
    }

    public async Task<Student?> GetStudentByUserIdAsync(int userId)
    {
        return await _context.Students
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.UserId == userId);
    }

    public async Task<StudentMasterList?> GetMasterStudentByIdAsync(
        int masterStudentId)
    {
        return await _context.StudentMasterLists
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.MasterStudentId == masterStudentId);
    }

    public async Task<IReadOnlyCollection<LeaveRequest>>
        GetRequestsByStudentIdAsync(int studentId)
    {
        return await BaseRequestQuery()
            .Where(x => x.StudentId == studentId)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync();
    }

    public async Task<IReadOnlyCollection<LeaveRequest>>
        GetRequestsByDepartmentIdsAsync(IReadOnlyCollection<int> departmentIds)
    {
        return await BaseRequestQuery()
            .Where(x => departmentIds.Contains(x.DepartmentId))
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync();
    }

    public async Task<IReadOnlyCollection<LeaveRequest>> GetAllRequestsAsync()
    {
        return await BaseRequestQuery()
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync();
    }

    public async Task<LeaveRequest?> GetRequestByIdAsync(int leaveRequestId)
    {
        return await BaseRequestQuery()
            .FirstOrDefaultAsync(x => x.LeaveRequestId == leaveRequestId);
    }

    public async Task AddRequestAsync(LeaveRequest request)
    {
        await _context.LeaveRequests.AddAsync(request);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateRequestAsync(LeaveRequest request)
    {
        _context.LeaveRequests.Update(request);
        await _context.SaveChangesAsync();
    }

    public async Task AddApprovalAsync(LeaveApproval approval)
    {
        await _context.LeaveApprovals.AddAsync(approval);
        await _context.SaveChangesAsync();
    }

    public async Task<IReadOnlyCollection<int>> GetAssignedDepartmentIdsAsync(
        int userId)
    {
        return await _context.DepartmentStaffAssignments
            .AsNoTracking()
            .Where(x => x.UserId == userId)
            .Select(x => x.DepartmentId)
            .Distinct()
            .ToListAsync();
    }

    public async Task AddAuditLogAsync(AuditLog auditLog)
    {
        await _context.AuditLogs.AddAsync(auditLog);
        await _context.SaveChangesAsync();
    }

    private IQueryable<LeaveRequest> BaseRequestQuery()
    {
        return _context.LeaveRequests
            .AsNoTracking()
            .Include(x => x.LeaveType)
            .Include(x => x.Student)
            .Include(x => x.Approvals)
            .AsSplitQuery();
    }
}
