using CampusServicePortal.Modules.Gym.Entities;
using CampusServicePortal.Modules.Gym.Interfaces.Repository;
using CampusServicePortal_TicUnicorns.Data;
using Microsoft.EntityFrameworkCore;
using GymEntity = CampusServicePortal.Modules.Gym.Entities.Gym;

namespace CampusServicePortal.Modules.Gym.Repositories;

public class GymRepository : IGymRepository
{
    private readonly CampusDbContext _context;

    public GymRepository(CampusDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<GymEntity>> GetAllGymsAsync()
    {
        return await _context.Gyms
            .AsNoTracking()
            .OrderBy(x => x.Name)
            .ToListAsync();
    }

    public async Task<GymEntity?> GetGymByIdAsync(int gymId)
    {
        return await _context.Gyms
            .FirstOrDefaultAsync(x => x.GymId == gymId);
    }

    public async Task<GymEntity> CreateGymAsync(GymEntity gym)
    {
        await _context.Gyms.AddAsync(gym);
        await _context.SaveChangesAsync();
        return gym;
    }

    public async Task<GymEntity?> UpdateGymAsync(GymEntity gym)
    {
        var existing = await _context.Gyms
            .FirstOrDefaultAsync(x => x.GymId == gym.GymId);

        if (existing == null)
        {
            return null;
        }

        existing.Name = gym.Name;
        existing.Location = gym.Location;
        existing.Capacity = gym.Capacity;
        existing.Description = gym.Description;
        existing.IsActive = gym.IsActive;

        await _context.SaveChangesAsync();
        return existing;
    }

    public async Task<bool> DeleteGymAsync(int gymId)
    {
        var gym = await _context.Gyms
            .FirstOrDefaultAsync(x => x.GymId == gymId);

        if (gym == null)
        {
            return false;
        }

        gym.IsActive = false;
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<IEnumerable<GymSlot>> GetSlotsByGymIdAsync(
        int gymId,
        DateTime? slotDate = null)
    {
        var query = _context.GymSlots
            .Include(x => x.Gym)
            .Where(x => x.GymId == gymId);

        if (slotDate.HasValue)
        {
            var date = slotDate.Value.Date;
            query = query.Where(x => x.SlotDate.Date == date);
        }

        return await query
            .OrderBy(x => x.SlotDate)
            .ThenBy(x => x.StartTime)
            .ToListAsync();
    }

    public async Task<GymSlot?> GetSlotByIdAsync(int slotId)
    {
        return await _context.GymSlots
            .Include(x => x.Gym)
            .FirstOrDefaultAsync(x => x.SlotId == slotId);
    }

    public async Task<GymSlot> CreateSlotAsync(GymSlot slot)
    {
        await _context.GymSlots.AddAsync(slot);
        await _context.SaveChangesAsync();
        return slot;
    }

    public async Task<GymSlot?> UpdateSlotAsync(GymSlot slot)
    {
        var existing = await _context.GymSlots
            .FirstOrDefaultAsync(x => x.SlotId == slot.SlotId);

        if (existing == null)
        {
            return null;
        }

        existing.SlotDate = slot.SlotDate;
        existing.StartTime = slot.StartTime;
        existing.EndTime = slot.EndTime;
        existing.MaxCapacity = slot.MaxCapacity;
        existing.IsAvailable = slot.IsAvailable;
        existing.RequiresPayment = slot.RequiresPayment;
        existing.FeeAmount = slot.FeeAmount;

        await _context.SaveChangesAsync();
        return existing;
    }

    public async Task<IEnumerable<GymBooking>> GetBookingsBySlotIdAsync(int slotId)
    {
        return await _context.GymBookings
            .Include(x => x.GymSlot)
                .ThenInclude(x => x!.Gym)
            .Where(x => x.SlotId == slotId)
            .OrderBy(x => x.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<GymBooking>> GetBookingsByUserIdAsync(int userId)
    {
        return await _context.GymBookings
            .Include(x => x.GymSlot)
                .ThenInclude(x => x!.Gym)
            .Where(x => x.UserId == userId)
            .OrderByDescending(x => x.BookingDate)
            .ThenByDescending(x => x.CreatedAt)
            .ToListAsync();
    }

    public async Task<GymBooking?> GetBookingByIdAsync(int bookingId)
    {
        return await _context.GymBookings
            .Include(x => x.GymSlot)
                .ThenInclude(x => x!.Gym)
            .FirstOrDefaultAsync(x => x.BookingId == bookingId);
    }

    public async Task<GymBooking?> GetBookingForUserAsync(int bookingId, int userId)
    {
        return await _context.GymBookings
            .Include(x => x.GymSlot)
                .ThenInclude(x => x!.Gym)
            .FirstOrDefaultAsync(x =>
                x.BookingId == bookingId &&
                x.UserId == userId);
    }

    public async Task<GymBooking?> GetExistingActiveBookingAsync(
        int slotId,
        int userId)
    {
        return await _context.GymBookings
            .FirstOrDefaultAsync(x =>
                x.SlotId == slotId &&
                x.UserId == userId &&
                (x.Status == "Held" || x.Status == "Confirmed"));
    }

    public async Task<GymBooking> CreateBookingAsync(GymBooking booking)
    {
        await _context.GymBookings.AddAsync(booking);
        await _context.SaveChangesAsync();
        return booking;
    }

    public async Task<GymBooking> UpdateBookingAsync(GymBooking booking)
    {
        _context.GymBookings.Update(booking);
        await _context.SaveChangesAsync();
        return booking;
    }
}
