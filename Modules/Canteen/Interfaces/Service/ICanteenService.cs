using CampusServicePortal_TicUnicorns.Modules.Canteen.DTOs;


namespace CampusServicePortal_TicUnicorns.Modules.Canteens.Interfaces.Service
{
    public interface ICanteenService
    {
        Task<IEnumerable<CanteenResponseDto>> GetAllAsync();

        Task<CanteenResponseDto?> GetByIdAsync(int canteenId);

        Task<IEnumerable<CanteenResponseDto>> GetByStudentIdAsync(int studentId);

        Task<CanteenResponseDto> CreateAsync(CreateCanteenDto dto);

        Task<CanteenResponseDto?> UpdateAsync(
            int canteenId,
            UpdateCanteenDto dto);

        Task<CanteenResponseDto?> UpdateStatusAsync(
            int canteenId,
            UpdateCanteenStatusDto dto);

        Task<bool> DeleteAsync(int canteenId);
    }
}