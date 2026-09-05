using CampusServicePortal_TicUnicorns.Modules.Laundry.DTOs;

namespace CampusServicePortal_TicUnicorns.Modules.Laundry.Interfaces.Service
{
    public interface ILaundryService
    {
        Task<IEnumerable<LaundryResponseDto>> GetAllAsync();

        Task<LaundryResponseDto?> GetByIdAsync(int laundryId);

        Task<IEnumerable<LaundryResponseDto>> GetByStudentIdAsync(int studentId);

        Task<LaundryResponseDto> CreateAsync(
            CreateLaundryDto dto);

        Task<bool> UpdateAsync(
            int laundryId,
            UpdateLaundryDto dto);

        Task<bool> UpdateStatusAsync(
            int laundryId,
            UpdateLaundryStatusDto dto);

        Task<bool> AssignAsync(
            int laundryId,
            AssignLaundryDto dto);

        Task<bool> DeleteAsync(int laundryId);
    }
}