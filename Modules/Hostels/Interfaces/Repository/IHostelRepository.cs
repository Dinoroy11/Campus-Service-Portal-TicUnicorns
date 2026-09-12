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

    Task<int> CountActiveBedsInRoomAsync(int roomId);
    Task<bool> BedNumberExistsInRoomAsync(int roomId, string bedNumber);
    Task<int?> GetHostelIdByBedIdAsync(int bedId);

    Task ExpireActiveHoldsAsync(DateTime utcNow);
    Task<List<int>> GetActiveHeldBedIdsAsync(DateTime utcNow);
    Task<HostelRoomHold?> GetHoldByIdAsync(int holdId);
    Task<HostelRoomHold?> GetHoldByApplicationIdAsync(int applicationId);
    Task<HostelRoomHold?> GetActiveHoldForBedAsync(int bedId, DateTime utcNow);
    Task<HostelRoomHold?> GetActiveHoldForStudentAsync(int studentId, DateTime utcNow);
    Task<HostelRoomHold> CreateHoldAsync(HostelRoomHold hold);
    Task UpdateHoldAsync(HostelRoomHold hold);

    Task<bool> HasActiveApplicationAsync(int studentId);
    Task<HostelApplication> CreateApplicationAsync(HostelApplication application);
    Task<HostelApplication?> GetApplicationByIdAsync(int applicationId);
    Task<List<HostelApplication>> GetApplicationsAsync();
    Task<List<HostelApplication>> GetApplicationsByStudentAsync(int studentId);
    Task UpdateApplicationAsync(HostelApplication application);

    Task<bool> HasActiveAllocationAsync(int studentId);
    Task<bool> IsBedActivelyAllocatedAsync(int bedId);
    Task<List<int>> GetActivelyAllocatedBedIdsAsync();
    Task<HostelAllocation> CreateAllocationAsync(HostelAllocation allocation);
    Task<HostelAllocation?> GetAllocationByIdAsync(int allocationId);
    Task<List<HostelAllocation>> GetAllocationsAsync();
    Task<List<HostelAllocation>> GetAllocationsByStudentAsync(int studentId);
    Task UpdateAllocationAsync(HostelAllocation allocation);
}
