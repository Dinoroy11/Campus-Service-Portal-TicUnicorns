using CampusServicePortal_TicUnicorns.Modules.Laundry.DTOs;
using CampusServicePortal_TicUnicorns.Modules.Laundry.Entities;
using CampusServicePortal_TicUnicorns.Modules.Laundry.Interfaces.Repository;
using CampusServicePortal_TicUnicorns.Modules.Laundry.Interfaces.Service;

namespace CampusServicePortal_TicUnicorns.Modules.Laundry.Services
{
    public class LaundryService : ILaundryService
    {
        private readonly ILaundryRepository _laundryRepository;

        public LaundryService(
            ILaundryRepository laundryRepository)
        {
            _laundryRepository = laundryRepository;
        }

        public async Task<IEnumerable<LaundryResponseDto>> GetAllAsync()
        {
            var laundryRequests =
                await _laundryRepository.GetAllAsync();

            return laundryRequests.Select(MapToResponseDto);
        }

        public async Task<LaundryResponseDto?> GetByIdAsync(
            int laundryId)
        {
            var laundry =
                await _laundryRepository.GetByIdAsync(laundryId);

            if (laundry == null)
                return null;

            return MapToResponseDto(laundry);
        }

        public async Task<IEnumerable<LaundryResponseDto>>
            GetByStudentIdAsync(int studentId)
        {
            var laundryRequests =
                await _laundryRepository.GetByStudentIdAsync(studentId);

            return laundryRequests.Select(MapToResponseDto);
        }

        public async Task<LaundryResponseDto> CreateAsync(
            CreateLaundryDto dto)
        {
            var laundry = new LaundryEntities
            {
                StudentId = dto.StudentId,
                ServiceType = dto.ServiceType,
                Quantity = dto.Quantity,
                PickupMethod = dto.PickupMethod,
                TotalAmount = 0,
                Status = "Pending",
                RequestedAt = DateTime.UtcNow
            };

            var createdLaundry =
                await _laundryRepository.CreateAsync(laundry);

            return MapToResponseDto(createdLaundry);
        }

        public async Task<bool> UpdateAsync(
            int laundryId,
            UpdateLaundryDto dto)
        {
            var laundry =
                await _laundryRepository.GetByIdAsync(laundryId);

            if (laundry == null)
                return false;

            laundry.ServiceType = dto.ServiceType;
            laundry.Quantity = dto.Quantity;
            laundry.PickupMethod = dto.PickupMethod;

            return await _laundryRepository.UpdateAsync(laundry);
        }

        public async Task<bool> UpdateStatusAsync(
            int laundryId,
            UpdateLaundryStatusDto dto)
        {
            var laundry =
                await _laundryRepository.GetByIdAsync(laundryId);

            if (laundry == null)
                return false;

            return await _laundryRepository.UpdateStatusAsync(
                laundryId,
                dto.Status);
        }

        public async Task<bool> AssignAsync(
            int laundryId,
            AssignLaundryDto dto)
        {
            var laundry =
                await _laundryRepository.GetByIdAsync(laundryId);

            if (laundry == null)
                return false;

            return await _laundryRepository.AssignAsync(
                laundryId,
                dto.AssignedTo);
        }

        public async Task<bool> DeleteAsync(int laundryId)
        {
            var laundry =
                await _laundryRepository.GetByIdAsync(laundryId);

            if (laundry == null)
                return false;

            return await _laundryRepository.DeleteAsync(laundryId);
        }

        private static LaundryResponseDto MapToResponseDto(
            LaundryEntities laundry)
        {
            return new LaundryResponseDto
            {
                LaundryId = laundry.LaundryId,
                StudentId = laundry.StudentId,
                ServiceType = laundry.ServiceType,
                Quantity = laundry.Quantity,
                PickupMethod = laundry.PickupMethod,
                TotalAmount = laundry.TotalAmount,
                Status = laundry.Status,
                RequestedAt = laundry.RequestedAt,
                ReadyAt = laundry.ReadyAt,
                AssignedTo = laundry.AssignedTo,
                CollectedAt = laundry.CollectedAt
            };
        }
    }
}