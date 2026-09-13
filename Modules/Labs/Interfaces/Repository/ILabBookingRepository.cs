using CampusServicePortal.Modules.Labs.Entities;

namespace CampusServicePortal.Modules.Labs.Interfaces.Repository;

public interface ILabBookingRepository
{
    Task<List<LabBooking>> GetAllAsync();

    Task<List<LabBooking>> GetByLabAndDateAsync(
        int labId,
        DateTime bookingDate);

    Task<List<LabBooking>> GetByTimeSlotAndDateAsync(
        int labId,
        int timeSlotId,
        DateTime bookingDate);

    Task<List<LabBooking>> GetOverlappingAsync(
        int labId,
        DateTime bookingDate,
        TimeSpan startTime,
        TimeSpan endTime);

    Task<List<LabBooking>> GetByStudentIdAsync(
        int studentId);

    Task<LabBooking?> GetByIdAsync(int labBookingId);

    Task<List<LabBooking>> GetActiveUpcomingBySeatAsync(
        int labSeatId,
        DateTime nowUtc);

    Task<LabBooking> CreateAsync(LabBooking booking);

    Task UpdateAsync(LabBooking booking);

    Task<bool> IsSeatBookedAsync(
        int labSeatId,
        int timeSlotId,
        DateTime bookingDate);
}