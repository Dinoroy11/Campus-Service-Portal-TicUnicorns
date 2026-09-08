using LeaveEntity = CampusServicePortal.Modules.Leave.Entities.Leave;
using CampusServicePortal.Modules.Leave.Interfaces.Repository;
using CampusServicePortal_TicUnicorns.Data;
using Microsoft.EntityFrameworkCore;

namespace CampusServicePortal.Modules.Leave.Repositories
{
    public class LeaveRepository : ILeaveRepository
    {
        private readonly CampusDbContext _context;

        public LeaveRepository(CampusDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<LeaveEntity>> GetAllLeavesAsync()
        {
            return await _context.Leaves
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<LeaveEntity?> GetLeaveByIdAsync(int leaveId)
        {
            return await _context.Leaves
                .FirstOrDefaultAsync(l => l.LeaveId == leaveId);
        }

        public async Task<LeaveEntity> CreateLeaveAsync(LeaveEntity leave)
        {
            leave.CreatedAt = DateTime.UtcNow;
            leave.Status = "Pending";

            await _context.Leaves.AddAsync(leave);
            await _context.SaveChangesAsync();

            return leave;
        }

        public async Task<LeaveEntity?> UpdateLeaveAsync(LeaveEntity leave)
        {
            var existingLeave = await _context.Leaves
                .FirstOrDefaultAsync(l => l.LeaveId == leave.LeaveId);

            if (existingLeave == null)
            {
                return null;
            }

            existingLeave.UserId = leave.UserId;
            existingLeave.LeaveType = leave.LeaveType;
            existingLeave.StartDate = leave.StartDate;
            existingLeave.EndDate = leave.EndDate;
            existingLeave.Reason = leave.Reason;
            existingLeave.Status = leave.Status;

            await _context.SaveChangesAsync();

            return existingLeave;
        }

        public async Task<bool> DeleteLeaveAsync(int leaveId)
        {
            var leave = await _context.Leaves
                .FirstOrDefaultAsync(l => l.LeaveId == leaveId);

            if (leave == null)
            {
                return false;
            }

            _context.Leaves.Remove(leave);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}