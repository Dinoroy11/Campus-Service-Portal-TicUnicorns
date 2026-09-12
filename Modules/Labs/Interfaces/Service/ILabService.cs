using CampusServicePortal.Modules.Labs.DTOs;

namespace CampusServicePortal.Modules.Labs.Interfaces.Service;

public interface ILabService
{
    Task<List<LabDto>> GetAllLabsAsync();

    Task<LabDto?> GetLabByIdAsync(int labId);

    Task<LabDto> CreateLabAsync(CreateLabDto dto);

    Task UpdateLabAsync(int labId, UpdateLabDto dto);

    Task<List<LabSeatDto>> GetSeatsByLabIdAsync(int labId);

    Task<LabSeatDto> CreateSeatAsync(CreateLabSeatDto dto);

    Task<LabSeatStatusUpdateResultDto> UpdateSeatStatusAsync(
        int labSeatId,
        UpdateLabSeatStatusDto dto);

    Task<List<LabTimeSlotDto>> GetTimeSlotsByLabIdAsync(int labId);

    Task<LabTimeSlotDto> CreateTimeSlotAsync(
        CreateLabTimeSlotDto dto);

    Task<LabAvailabilityDto> GetAvailabilityAsync(
        int labId,
        int timeSlotId,
        DateTime bookingDate,
        TimeSpan? requestedStartTime = null,
        double? requestedHours = null);

    Task<LabBookingDto> CreateBookingAsync(
        CreateLabBookingDto dto);

    Task<LabBookingDto?> GetBookingByIdAsync(
        int labBookingId);

    Task CancelBookingAsync(int labBookingId);
}