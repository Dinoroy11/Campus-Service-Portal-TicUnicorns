using LeaveEntity = CampusServicePortal.Modules.Leave.Entities.Leave;
using CampusServicePortal.Modules.Leave.Interfaces.Repository;

namespace CampusServicePortal.Modules.Leave.Repositories
{
    public class LeaveRepository : ILeaveRepository
    {
        private readonly List<LeaveEntity> _leaves = new();

        public Task<IEnumerable<LeaveEntity>> GetAllLeavesAsync()
        {
            return Task.FromResult<IEnumerable<LeaveEntity>>(_leaves);
        }

        public Task<LeaveEntity?> GetLeaveByIdAsync(int leaveId)
        {
            var leave = _leaves.FirstOrDefault(x => x.LeaveId == leaveId);

            return Task.FromResult(leave);
        }

        public Task<LeaveEntity> CreateLeaveAsync(LeaveEntity leave)
        {
            leave.LeaveId = _leaves.Count + 1;
            leave.CreatedAt = DateTime.UtcNow;
            leave.Status = "Pending";

            _leaves.Add(leave);

            return Task.FromResult(leave);
        }

        public Task<LeaveEntity?> UpdateLeaveAsync(LeaveEntity leave)
        {
            var existingLeave = _leaves
                .FirstOrDefault(x => x.LeaveId == leave.LeaveId);

            if (existingLeave == null)
            {
                return Task.FromResult<LeaveEntity?>(null);
            }

            existingLeave.UserId = leave.UserId;
            existingLeave.LeaveType = leave.LeaveType;
            existingLeave.StartDate = leave.StartDate;
            existingLeave.EndDate = leave.EndDate;
            existingLeave.Reason = leave.Reason;
            existingLeave.Status = leave.Status;

            return Task.FromResult<LeaveEntity?>(existingLeave);
        }

        public Task<bool> DeleteLeaveAsync(int leaveId)
        {
            var leave = _leaves
                .FirstOrDefault(x => x.LeaveId == leaveId);

            if (leave == null)
            {
                return Task.FromResult(false);
            }

            _leaves.Remove(leave);

            return Task.FromResult(true);
        }
    }
}