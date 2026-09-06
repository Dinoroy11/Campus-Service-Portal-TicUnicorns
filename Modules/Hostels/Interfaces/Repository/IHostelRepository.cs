using CampusServicePortal.Modules.Hostels.Entities;

namespace CampusServicePortal.Modules.Hostels.Repositories;

public interface IHostelRepository
{
    Task<List<Hostel>> GetHostelsAsync();

    Task<Hostel?> GetHostelBlueprintAsync(int hostelId);

    Task<Hostel> CreateHostelAsync(Hostel hostel);

    Task<Floor> CreateFloorAsync(Floor floor);

    Task<Room> CreateRoomAsync(Room room);

    Task<RoomBed> CreateRoomBedAsync(RoomBed roomBed);

    Task<Hostel?> GetHostelByIdAsync(int hostelId);

    Task<Floor?> GetFloorByIdAsync(int floorId);

    Task<Room?> GetRoomByIdAsync(int roomId);

    Task<RoomBed?> GetRoomBedByIdAsync(int bedId);

    Task UpdateHostelAsync(Hostel hostel);

    Task UpdateFloorAsync(Floor floor);

    Task UpdateRoomAsync(Room room);

    Task UpdateRoomBedAsync(RoomBed roomBed);
}