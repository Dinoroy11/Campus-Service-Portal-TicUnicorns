using CampusServicePortal.Modules.Leave.DTOs;
using CampusServicePortal.Modules.Leave.Entities;
using CampusServicePortal.Modules.Leave.Interfaces.Repository;
using CampusServicePortal.Modules.Leave.Interfaces.Service;
using LeaveEntity = CampusServicePortal.Modules.Leave.Entities.Leave;

namespace CampusServicePortal.Modules.Leave.Services
{
    public class LeaveService : ILeaveService
    {
        private readonly ILeaveRepository _leaveRepository;

        public LeaveService(ILeaveRepository leaveRepository)
        {
            _leaveRepository = leaveRepository;
        }

        public async Task<IEnumerable<LeaveDto>> GetAllLeavesAsync()
        {
            var leaves = await _leaveRepository.GetAllLeavesAsync();

            return leaves.Select(leave => new LeaveDto
            {
                LeaveId = leave.LeaveId,
                UserId = leave.UserId,
                LeaveType = leave.LeaveType,
                StartDate = leave.StartDate,
                EndDate = leave.EndDate,
                Reason = leave.Reason,
                Status = leave.Status,
                CreatedAt = leave.CreatedAt
            });
        }

        public async Task<LeaveDto?> GetLeaveByIdAsync(int leaveId)
        {
            var leave = await _leaveRepository.GetLeaveByIdAsync(leaveId);

            if (leave == null)
            {
                return null;
            }

            return new LeaveDto
            {
                LeaveId = leave.LeaveId,
                UserId = leave.UserId,
                LeaveType = leave.LeaveType,
                StartDate = leave.StartDate,
                EndDate = leave.EndDate,
                Reason = leave.Reason,
                Status = leave.Status,
                CreatedAt = leave.CreatedAt
            };
        }

        public async Task<LeaveDto> CreateLeaveAsync(CreateLeaveDto leaveDto)
        {
            var leave = new LeaveEntity
            {
                UserId = leaveDto.UserId,
                LeaveType = leaveDto.LeaveType,
                StartDate = leaveDto.StartDate,
                EndDate = leaveDto.EndDate,
                Reason = leaveDto.Reason,
                Status = "Pending",
                CreatedAt = DateTime.UtcNow
            };

            var createdLeave = await _leaveRepository.CreateLeaveAsync(leave);

            return new LeaveDto
            {
                LeaveId = createdLeave.LeaveId,
                UserId = createdLeave.UserId,
                LeaveType = createdLeave.LeaveType,
                StartDate = createdLeave.StartDate,
                EndDate = createdLeave.EndDate,
                Reason = createdLeave.Reason,
                Status = createdLeave.Status,
                CreatedAt = createdLeave.CreatedAt
            };
        }

        public async Task<LeaveDto?> UpdateLeaveAsync(
            int leaveId,
            UpdateLeaveDto leaveDto)
        {
            var existingLeave =
                await _leaveRepository.GetLeaveByIdAsync(leaveId);

            if (existingLeave == null)
            {
                return null;
            }

            existingLeave.LeaveType = leaveDto.LeaveType;
            existingLeave.StartDate = leaveDto.StartDate;
            existingLeave.EndDate = leaveDto.EndDate;
            existingLeave.Reason = leaveDto.Reason;
            existingLeave.Status = leaveDto.Status;

            var updatedLeave =
                await _leaveRepository.UpdateLeaveAsync(existingLeave);

            if (updatedLeave == null)
            {
                return null;
            }

            return new LeaveDto
            {
                LeaveId = updatedLeave.LeaveId,
                UserId = updatedLeave.UserId,
                LeaveType = updatedLeave.LeaveType,
                StartDate = updatedLeave.StartDate,
                EndDate = updatedLeave.EndDate,
                Reason = updatedLeave.Reason,
                Status = updatedLeave.Status,
                CreatedAt = updatedLeave.CreatedAt
            };
        }

        public async Task<bool> DeleteLeaveAsync(int leaveId)
        {
            return await _leaveRepository.DeleteLeaveAsync(leaveId);
        }
    }
}