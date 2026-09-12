using CampusServicePortal.Modules.Hostels.Entities;
using CampusServicePortal_TicUnicorns.Data;
using Microsoft.EntityFrameworkCore;

namespace CampusServicePortal.Modules.Hostels.Repositories;

public class HostelRepository : IHostelRepository
{
    private readonly CampusDbContext _context;

    public HostelRepository(CampusDbContext context)
    {
        _context = context;
    }

    public async Task<List<Hostel>> GetHostelsAsync()
    {
        return await _context.Hostels
            .AsNoTracking()
            .Where(h => h.IsActive)
            .ToListAsync();
    }

    public async Task<Hostel?> GetHostelBlueprintAsync(int hostelId)
    {
        return await _context.Hostels
            .AsNoTracking()
            .Where(h => h.HostelId == hostelId && h.IsActive)
            .Include(h => h.Floors)
                .ThenInclude(f => f.Rooms)
                    .ThenInclude(r => r.RoomBeds)
            .FirstOrDefaultAsync();
    }

    public async Task<Hostel> CreateHostelAsync(Hostel hostel)
    {
        _context.Hostels.Add(hostel);
        await _context.SaveChangesAsync();
        return hostel;
    }

    public async Task<Floor> CreateFloorAsync(Floor floor)
    {
        _context.Floors.Add(floor);
        await _context.SaveChangesAsync();
        return floor;
    }

    public async Task<Room> CreateRoomAsync(Room room)
    {
        _context.Rooms.Add(room);
        await _context.SaveChangesAsync();
        return room;
    }

    public async Task<RoomBed> CreateRoomBedAsync(RoomBed roomBed)
    {
        _context.RoomBeds.Add(roomBed);
        await _context.SaveChangesAsync();
        return roomBed;
    }

    public async Task<Hostel?> GetHostelByIdAsync(int hostelId)
    {
        return await _context.Hostels
            .FirstOrDefaultAsync(h => h.HostelId == hostelId);
    }

    public async Task<Floor?> GetFloorByIdAsync(int floorId)
    {
        return await _context.Floors
            .FirstOrDefaultAsync(f => f.FloorId == floorId);
    }

    public async Task<Room?> GetRoomByIdAsync(int roomId)
    {
        return await _context.Rooms
            .FirstOrDefaultAsync(r => r.RoomId == roomId);
    }

    public async Task<RoomBed?> GetRoomBedByIdAsync(int bedId)
    {
        return await _context.RoomBeds
            .FirstOrDefaultAsync(b => b.BedId == bedId);
    }

    public async Task UpdateHostelAsync(Hostel hostel)
    {
        _context.Hostels.Update(hostel);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateFloorAsync(Floor floor)
    {
        _context.Floors.Update(floor);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateRoomAsync(Room room)
    {
        _context.Rooms.Update(room);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateRoomBedAsync(RoomBed roomBed)
    {
        _context.RoomBeds.Update(roomBed);
        await _context.SaveChangesAsync();
    }

    public async Task<int> CountActiveBedsInRoomAsync(int roomId)
    {
        return await _context.RoomBeds
            .CountAsync(x => x.RoomId == roomId && x.IsActive);
    }

    public async Task<bool> BedNumberExistsInRoomAsync(
        int roomId,
        string bedNumber)
    {
        return await _context.RoomBeds
            .AnyAsync(x =>
                x.RoomId == roomId &&
                x.BedNumber == bedNumber);
    }

    public async Task<int?> GetHostelIdByBedIdAsync(int bedId)
    {
        return await _context.RoomBeds
            .Where(x => x.BedId == bedId)
            .Select(x => (int?)x.Room.Floor.HostelId)
            .FirstOrDefaultAsync();
    }

    public async Task ExpireActiveHoldsAsync(DateTime utcNow)
    {
        var expiredHolds = await _context.HostelRoomHolds
            .Where(x =>
                x.Status == "Active" &&
                x.ExpiresAt <= utcNow)
            .ToListAsync();

        if (expiredHolds.Count == 0)
        {
            return;
        }

        foreach (var hold in expiredHolds)
        {
            hold.Status = "Expired";
        }

        await _context.SaveChangesAsync();
    }

    public async Task<List<int>> GetActiveHeldBedIdsAsync(DateTime utcNow)
    {
        return await _context.HostelRoomHolds
            .AsNoTracking()
            .Where(x =>
                x.Status == "Active" &&
                x.ExpiresAt > utcNow)
            .Select(x => x.RoomBedId)
            .Distinct()
            .ToListAsync();
    }

    public async Task<HostelRoomHold?> GetHoldByIdAsync(int holdId)
    {
        return await _context.HostelRoomHolds
            .FirstOrDefaultAsync(x => x.HostelRoomHoldId == holdId);
    }

    public async Task<HostelRoomHold?> GetHoldByApplicationIdAsync(
        int applicationId)
    {
        return await _context.HostelRoomHolds
            .Where(x => x.ApplicationId == applicationId)
            .OrderByDescending(x => x.HeldAt)
            .FirstOrDefaultAsync();
    }

    public async Task<HostelRoomHold?> GetActiveHoldForBedAsync(
        int bedId,
        DateTime utcNow)
    {
        return await _context.HostelRoomHolds
            .FirstOrDefaultAsync(x =>
                x.RoomBedId == bedId &&
                x.Status == "Active" &&
                x.ExpiresAt > utcNow);
    }

    public async Task<HostelRoomHold?> GetActiveHoldForStudentAsync(
        int studentId,
        DateTime utcNow)
    {
        return await _context.HostelRoomHolds
            .FirstOrDefaultAsync(x =>
                x.StudentId == studentId &&
                x.Status == "Active" &&
                x.ExpiresAt > utcNow);
    }

    public async Task<HostelRoomHold> CreateHoldAsync(HostelRoomHold hold)
    {
        _context.HostelRoomHolds.Add(hold);
        await _context.SaveChangesAsync();
        return hold;
    }

    public async Task UpdateHoldAsync(HostelRoomHold hold)
    {
        _context.HostelRoomHolds.Update(hold);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> HasActiveApplicationAsync(int studentId)
    {
        return await _context.HostelApplications
            .AnyAsync(x =>
                x.StudentId == studentId &&
                (x.Status == "Pending" ||
                 x.Status == "InProgress" ||
                 x.Status == "Approved"));
    }

    public async Task<HostelApplication> CreateApplicationAsync(
        HostelApplication application)
    {
        _context.HostelApplications.Add(application);
        await _context.SaveChangesAsync();
        return application;
    }

    public async Task<HostelApplication?> GetApplicationByIdAsync(
        int applicationId)
    {
        return await _context.HostelApplications
            .FirstOrDefaultAsync(x =>
                x.HostelApplicationId == applicationId);
    }

    public async Task<List<HostelApplication>> GetApplicationsAsync()
    {
        return await _context.HostelApplications
            .AsNoTracking()
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync();
    }

    public async Task<List<HostelApplication>> GetApplicationsByStudentAsync(
        int studentId)
    {
        return await _context.HostelApplications
            .AsNoTracking()
            .Where(x => x.StudentId == studentId)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync();
    }

    public async Task UpdateApplicationAsync(HostelApplication application)
    {
        _context.HostelApplications.Update(application);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> HasActiveAllocationAsync(int studentId)
    {
        return await _context.HostelAllocations
            .AnyAsync(x =>
                x.StudentId == studentId &&
                x.Status == "Active");
    }

    public async Task<bool> IsBedActivelyAllocatedAsync(int bedId)
    {
        return await _context.HostelAllocations
            .AnyAsync(x =>
                x.BedId == bedId &&
                x.Status == "Active");
    }

    public async Task<List<int>> GetActivelyAllocatedBedIdsAsync()
    {
        return await _context.HostelAllocations
            .AsNoTracking()
            .Where(x => x.Status == "Active")
            .Select(x => x.BedId)
            .Distinct()
            .ToListAsync();
    }

    public async Task<HostelAllocation> CreateAllocationAsync(
        HostelAllocation allocation)
    {
        _context.HostelAllocations.Add(allocation);
        await _context.SaveChangesAsync();
        return allocation;
    }

    public async Task<HostelAllocation?> GetAllocationByIdAsync(
        int allocationId)
    {
        return await _context.HostelAllocations
            .FirstOrDefaultAsync(x =>
                x.HostelAllocationId == allocationId);
    }

    public async Task<List<HostelAllocation>> GetAllocationsAsync()
    {
        return await _context.HostelAllocations
            .AsNoTracking()
            .OrderByDescending(x => x.AllocatedAt)
            .ToListAsync();
    }

    public async Task<List<HostelAllocation>> GetAllocationsByStudentAsync(
        int studentId)
    {
        return await _context.HostelAllocations
            .AsNoTracking()
            .Where(x => x.StudentId == studentId)
            .OrderByDescending(x => x.AllocatedAt)
            .ToListAsync();
    }

    public async Task UpdateAllocationAsync(HostelAllocation allocation)
    {
        _context.HostelAllocations.Update(allocation);
        await _context.SaveChangesAsync();
    }
}
