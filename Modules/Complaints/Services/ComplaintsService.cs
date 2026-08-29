using CampusServicePortal_TicUnicorns.Modules.Complaints.DTOs;
using CampusServicePortal_TicUnicorns.Modules.Complaints.Entities;
using CampusServicePortal_TicUnicorns.Modules.Complaints.Interfaces.Repository;
using CampusServicePortal_TicUnicorns.Modules.Complaints.Interfaces.Service;

namespace CampusServicePortal_TicUnicorns.Modules.Complaints.Services
{
    public class ComplaintsService : IComplaintsService
    {
        private readonly IComplaintsRepository _complaintsRepository;

        public ComplaintsService(IComplaintsRepository complaintsRepository)
        {
            _complaintsRepository = complaintsRepository;
        }

        public async Task<IEnumerable<ComplaintResponseDto>> GetAllAsync()
        {
            var complaints = await _complaintsRepository.GetAllAsync();

            return complaints.Select(MapToResponseDto);
        }

        public async Task<ComplaintResponseDto?> GetByIdAsync(int complaintId)
        {
            var complaint = await _complaintsRepository.GetByIdAsync(complaintId);

            if (complaint == null)
                return null;

            return MapToResponseDto(complaint);
        }

        public async Task<IEnumerable<ComplaintResponseDto>> GetByStudentIdAsync(
            int studentId)
        {
            var complaints =
                await _complaintsRepository.GetByStudentIdAsync(studentId);

            return complaints.Select(MapToResponseDto);
        }

        public async Task<ComplaintResponseDto> CreateAsync(
            CreateComplaintDto dto)
        {
            var complaint = new ComplaintsEntities
            {
                StudentId = dto.StudentId,
                Subject = dto.Subject,
                Description = dto.Description,
                Category = dto.Category,
                Priority = dto.Priority,
                Status = "Pending",
                CreatedAt = DateTime.UtcNow
            };

            var createdComplaint =
                await _complaintsRepository.CreateAsync(complaint);

            return MapToResponseDto(createdComplaint);
        }

        public async Task<bool> UpdateAsync(
            int complaintId,
            UpdateComplaintDto dto)
        {
            var complaint =
                await _complaintsRepository.GetByIdAsync(complaintId);

            if (complaint == null)
                return false;

            complaint.Subject = dto.Subject;
            complaint.Description = dto.Description;
            complaint.Category = dto.Category;
            complaint.Priority = dto.Priority;
            complaint.UpdatedAt = DateTime.UtcNow;

            return await _complaintsRepository.UpdateAsync(complaint);
        }

        public async Task<bool> UpdateStatusAsync(
            int complaintId,
            UpdateComplaintStatusDto dto)
        {
            var complaint =
                await _complaintsRepository.GetByIdAsync(complaintId);

            if (complaint == null)
                return false;

            return await _complaintsRepository.UpdateStatusAsync(
                complaintId,
                dto.Status,
                dto.Resolution);
        }

        public async Task<bool> AssignAsync(
            int complaintId,
            AssignComplaintDto dto)
        {
            var complaint =
                await _complaintsRepository.GetByIdAsync(complaintId);

            if (complaint == null)
                return false;

            return await _complaintsRepository.AssignAsync(
                complaintId,
                dto.AssignedTo);
        }

        public async Task<bool> DeleteAsync(int complaintId)
        {
            var complaint =
                await _complaintsRepository.GetByIdAsync(complaintId);

            if (complaint == null)
                return false;

            return await _complaintsRepository.DeleteAsync(complaintId);
        }

        private static ComplaintResponseDto MapToResponseDto(
            ComplaintsEntities complaint)
        {
            return new ComplaintResponseDto
            {
                ComplaintId = complaint.ComplaintId,
                StudentId = complaint.StudentId,
                Subject = complaint.Subject,
                Description = complaint.Description,
                Category = complaint.Category,
                Status = complaint.Status,
                Priority = complaint.Priority,
                AssignedTo = complaint.AssignedTo,
                Resolution = complaint.Resolution,
                CreatedAt = complaint.CreatedAt,
                UpdatedAt = complaint.UpdatedAt
            };
        }
    }
}