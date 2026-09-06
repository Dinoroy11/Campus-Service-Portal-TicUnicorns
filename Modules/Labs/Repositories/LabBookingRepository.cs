using CampusServicePortal.Modules.Labs.Entities;
using CampusServicePortal.Modules.Labs.Interfaces.Repository;
using CampusServicePortal_TicUnicorns.Data;
using Microsoft.EntityFrameworkCore;
 

namespace CampusServicePortal.Modules.Labs.Repositories;

public class LabBookingRepository : ILabBookingRepository
{
    private readonly CampusDbContext _context;

    public LabBookingRepository(CampusDbContext context)
    {
        _context = context;
    }

    public async Task<List<LabBooking>> GetByLabAndDateAsync(
        int labId,
        DateTime bookingDate)
    {
        return await _context.Set<LabBooking>()
            .AsNoTracking()
            .Where(x =>
                x.LabId == labId &&
                x.BookingDate.Date == bookingDate.Date)
            .ToListAsync();
    }

    public async Task<List<LabBooking>> GetByTimeSlotAndDateAsync(
        int labId,
        int timeSlotId,
        DateTime bookingDate)
    {
        return await _context.Set<LabBooking>()
            .AsNoTracking()
            .Where(x =>
                x.LabId == labId &&
                x.TimeSlotId == timeSlotId &&
                x.BookingDate.Date == bookingDate.Date)
            .ToListAsync();
    }

    public async Task<LabBooking?> GetByIdAsync(int labBookingId)
    {
        return await _context.Set<LabBooking>()
            .FirstOrDefaultAsync(x =>
                x.LabBookingId == labBookingId);
    }

    public async Task<LabBooking> CreateAsync(LabBooking booking)
    {
        await _context.Set<LabBooking>().AddAsync(booking);
        await _context.SaveChangesAsync();

        return booking;
    }

    public async Task UpdateAsync(LabBooking booking)
    {
        _context.Set<LabBooking>().Update(booking);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> IsSeatBookedAsync(
        int labSeatId,
        int timeSlotId,
        DateTime bookingDate)
    {
        return await _context.Set<LabBooking>()
            .AnyAsync(x =>
                x.LabSeatId == labSeatId &&
                x.TimeSlotId == timeSlotId &&
                x.BookingDate.Date == bookingDate.Date &&
                x.Status == Enums.LabBookingStatus.Booked);
    }
}