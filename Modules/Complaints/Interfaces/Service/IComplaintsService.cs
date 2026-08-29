using CampusServicePortal_TicUnicorns.Modules.Complaints.DTOs;

namespace CampusServicePortal_TicUnicorns.Modules.Complaints.Interfaces.Service
{
    public interface IComplaintsService
    {
        Task<IEnumerable<ComplaintResponseDto>> GetAllAsync();

        Task<ComplaintResponseDto?> GetByIdAsync(int complaintId);

        Task<IEnumerable<ComplaintResponseDto>> GetByStudentIdAsync(int studentId);

        Task<ComplaintResponseDto> CreateAsync(CreateComplaintDto dto);

        Task<bool> UpdateAsync(
            int complaintId,
            UpdateComplaintDto dto);

        Task<bool> UpdateStatusAsync(
            int complaintId,
            UpdateComplaintStatusDto dto);

        Task<bool> AssignAsync(
            int complaintId,
            AssignComplaintDto dto);

        Task<bool> DeleteAsync(int complaintId);
    }
}