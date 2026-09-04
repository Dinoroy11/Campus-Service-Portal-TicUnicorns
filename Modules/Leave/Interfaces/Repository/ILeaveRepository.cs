using LeaveEntity = CampusServicePortal.Modules.Leave.Entities.Leave;

namespace CampusServicePortal.Modules.Leave.Interfaces.Repository
{
    public interface ILeaveRepository
    {
        Task<IEnumerable<LeaveEntity>> GetAllLeavesAsync();

        Task<LeaveEntity?> GetLeaveByIdAsync(int leaveId);

        Task<LeaveEntity> CreateLeaveAsync(LeaveEntity leave);

        Task<LeaveEntity?> UpdateLeaveAsync(LeaveEntity leave);

        Task<bool> DeleteLeaveAsync(int leaveId);
    }
}