using CampusServicePortal_TicUnicorns.Modules.Canteen.DTOs;
using CampusServicePortal_TicUnicorns.Modules.Canteen.Entities;
using CampusServicePortal_TicUnicorns.Modules.Canteens.Interfaces.Repository;
using CampusServicePortal_TicUnicorns.Modules.Canteens.Interfaces.Service;

namespace CampusServicePortal_TicUnicorns.Modules.Canteens.Services
{
    public class CanteenService : ICanteenService
    {
        private readonly ICanteenRepository _repository;

        public CanteenService(ICanteenRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<CanteenResponseDto>> GetAllAsync()
        {
            var canteens = await _repository.GetAllAsync();

            return canteens.Select(MapToResponse);
        }

        public async Task<CanteenResponseDto?> GetByIdAsync(int canteenId)
        {
            var canteen = await _repository.GetByIdAsync(canteenId);

            return canteen == null ? null : MapToResponse(canteen);
        }

        public async Task<IEnumerable<CanteenResponseDto>> GetByStudentIdAsync(
            int studentId)
        {
            var canteens = await _repository.GetByStudentIdAsync(studentId);

            return canteens.Select(MapToResponse);
        }

        public async Task<CanteenResponseDto> CreateAsync(
            CreateCanteenDto dto)
        {
            var canteen = new CanteenEntities
            {
                StudentId = dto.StudentId,
                FoodName = dto.FoodName,
                MealType = dto.MealType,
                Quantity = dto.Quantity,
                Price = dto.Price,
                OrderType = dto.OrderType,
                Status = "Pending",
                OrderedAt = DateTime.UtcNow
            };

            var created = await _repository.CreateAsync(canteen);

            return MapToResponse(created);
        }

        public async Task<CanteenResponseDto?> UpdateAsync(
            int canteenId,
            UpdateCanteenDto dto)
        {
            var canteen = new CanteenEntities
            {
                FoodName = dto.FoodName,
                MealType = dto.MealType,
                Quantity = dto.Quantity,
                Price = dto.Price,
                OrderType = dto.OrderType
            };

            var updated = await _repository.UpdateAsync(
                canteenId,
                canteen);

            return updated == null ? null : MapToResponse(updated);
        }

        public async Task<CanteenResponseDto?> UpdateStatusAsync(
            int canteenId,
            UpdateCanteenStatusDto dto)
        {
            var updated = await _repository.UpdateStatusAsync(
                canteenId,
                dto.Status);

            return updated == null ? null : MapToResponse(updated);
        }

        public async Task<bool> DeleteAsync(int canteenId)
        {
            return await _repository.DeleteAsync(canteenId);
        }

        private static CanteenResponseDto MapToResponse(
            CanteenEntities canteen)
        {
            return new CanteenResponseDto
            {
                CanteenId = canteen.CanteenId,
                StudentId = canteen.StudentId,
                FoodName = canteen.FoodName,
                MealType = canteen.MealType,
                Quantity = canteen.Quantity,
                Price = canteen.Price,
                OrderType = canteen.OrderType,
                Status = canteen.Status,
                OrderedAt = canteen.OrderedAt,
                ReadyAt = canteen.ReadyAt,
                DeliveredAt = canteen.DeliveredAt,
                CollectedAt = canteen.CollectedAt
            };
        }
    }
}