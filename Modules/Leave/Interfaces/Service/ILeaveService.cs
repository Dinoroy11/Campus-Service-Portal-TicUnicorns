using CampusServicePortal.Modules.Leave.DTOs;

namespace CampusServicePortal.Modules.Leave.Interfaces.Service
{
    public interface ILeaveService
    {
        Task<IEnumerable<LeaveDto>> GetAllLeavesAsync();

        Task<LeaveDto?> GetLeaveByIdAsync(int leaveId);

        Task<LeaveDto> CreateLeaveAsync(CreateLeaveDto leaveDto);

        Task<LeaveDto?> UpdateLeaveAsync(int leaveId, UpdateLeaveDto leaveDto);

        Task<bool> DeleteLeaveAsync(int leaveId);
    }
}