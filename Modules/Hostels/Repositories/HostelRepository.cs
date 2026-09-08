using CampusServicePortal_TicUnicorns.Data;
using CampusServicePortal.Modules.Hostels.Entities;
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
            .Where(h => h.IsActive)
            .ToListAsync();
    }

    public async Task<Hostel?> GetHostelBlueprintAsync(int hostelId)
    {
        return await _context.Hostels
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
}