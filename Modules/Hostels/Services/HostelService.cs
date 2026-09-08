using CampusServicePortal.Modules.Hostels.DTOs;
using CampusServicePortal.Modules.Hostels.Entities;
using CampusServicePortal.Modules.Hostels.Repositories;

namespace CampusServicePortal.Modules.Hostels.Services;

public class HostelService : IHostelService
{
    private readonly IHostelRepository _hostelRepository;

    public HostelService(IHostelRepository hostelRepository)
    {
        _hostelRepository = hostelRepository;
    }

    public async Task<List<HostelBlueprintDto>> GetHostelsAsync()
    {
        var hostels = await _hostelRepository.GetHostelsAsync();

        return hostels.Select(h => new HostelBlueprintDto
        {
            HostelId = h.HostelId,
            HostelName = h.HostelName
        }).ToList();
    }

    public async Task<HostelBlueprintDto?> GetHostelBlueprintAsync(int hostelId)
    {
        var hostel = await _hostelRepository
            .GetHostelBlueprintAsync(hostelId);

        if (hostel == null)
            return null;

        return new HostelBlueprintDto
        {
            HostelId = hostel.HostelId,
            HostelName = hostel.HostelName,

            Floors = hostel.Floors.Select(f => new FloorBlueprintDto
            {
                FloorId = f.FloorId,
                FloorNumber = f.FloorNumber,
                Name = f.Name,

                Rooms = f.Rooms.Select(r => new RoomBlueprintDto
                {
                    RoomId = r.RoomId,
                    RoomNumber = r.RoomNumber,
                    Capacity = r.Capacity,
                    RoomType = r.RoomType,

                    IsAvailable = r.RoomBeds.Any(
                        b => b.IsActive && b.Status == "Available"),

                    Beds = r.RoomBeds
                        .Where(b => b.IsActive)
                        .Select(b => new BedBlueprintDto
                        {
                            BedId = b.BedId,
                            BedNumber = b.BedNumber,
                            Status = b.Status,
                            IsAvailable = b.Status == "Available"
                        })
                        .ToList()
                }).ToList()
            }).ToList()
        };
    }

    public async Task<HostelBlueprintDto> CreateHostelAsync(
        CreateHostelDto dto)
    {
        var hostel = new Hostel
        {
            UniversityId = dto.UniversityId,
            HostelName = dto.HostelName,
            HostelType = dto.HostelType,
            Description = dto.Description,
            IsActive = dto.IsActive
        };

        var createdHostel =
            await _hostelRepository.CreateHostelAsync(hostel);

        return new HostelBlueprintDto
        {
            HostelId = createdHostel.HostelId,
            HostelName = createdHostel.HostelName
        };
    }

    public async Task CreateFloorAsync(CreateFloorDto dto)
    {
        var floor = new Floor
        {
            HostelId = dto.HostelId,
            FloorNumber = dto.FloorNumber,
            Name = dto.Name,
            IsActive = dto.IsActive
        };

        await _hostelRepository.CreateFloorAsync(floor);
    }

    public async Task CreateRoomAsync(CreateRoomDto dto)
    {
        var room = new Room
        {
            FloorId = dto.FloorId,
            RoomNumber = dto.RoomNumber,
            Capacity = dto.Capacity,
            RoomType = dto.RoomType,
            IsActive = dto.IsActive
        };

        await _hostelRepository.CreateRoomAsync(room);
    }

    public async Task CreateRoomBedAsync(CreateRoomBedDto dto)
    {
        var roomBed = new RoomBed
        {
            RoomId = dto.RoomId,
            BedNumber = dto.BedNumber,
            Status = dto.Status,
            IsActive = dto.IsActive
        };

        await _hostelRepository.CreateRoomBedAsync(roomBed);
    }

    public async Task<bool> UpdateHostelAsync(
        int hostelId,
        UpdateHostelDto dto)
    {
        var hostel = await _hostelRepository
            .GetHostelByIdAsync(hostelId);

        if (hostel == null)
            return false;

        hostel.HostelName = dto.HostelName;
        hostel.HostelType = dto.HostelType;
        hostel.Description = dto.Description;
        hostel.IsActive = dto.IsActive;

        await _hostelRepository.UpdateHostelAsync(hostel);

        return true;
    }

    public async Task<bool> UpdateFloorAsync(
        int floorId,
        UpdateFloorDto dto)
    {
        var floor = await _hostelRepository
            .GetFloorByIdAsync(floorId);

        if (floor == null)
            return false;

        floor.FloorNumber = dto.FloorNumber;
        floor.Name = dto.Name;
        floor.IsActive = dto.IsActive;

        await _hostelRepository.UpdateFloorAsync(floor);

        return true;
    }

    public async Task<bool> UpdateRoomAsync(
        int roomId,
        UpdateRoomDto dto)
    {
        var room = await _hostelRepository
            .GetRoomByIdAsync(roomId);

        if (room == null)
            return false;

        room.RoomNumber = dto.RoomNumber;
        room.Capacity = dto.Capacity;
        room.RoomType = dto.RoomType;
        room.IsActive = dto.IsActive;

        await _hostelRepository.UpdateRoomAsync(room);

        return true;
    }

    public async Task<bool> UpdateRoomBedAsync(
        int bedId,
        UpdateRoomBedDto dto)
    {
        var roomBed = await _hostelRepository
            .GetRoomBedByIdAsync(bedId);

        if (roomBed == null)
            return false;

        roomBed.BedNumber = dto.BedNumber;
        roomBed.Status = dto.Status;
        roomBed.IsActive = dto.IsActive;

        await _hostelRepository.UpdateRoomBedAsync(roomBed);

        return true;
    }
}