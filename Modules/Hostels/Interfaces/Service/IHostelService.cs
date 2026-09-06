using CampusServicePortal.Modules.Hostels.DTOs;

namespace CampusServicePortal.Modules.Hostels.Services;

public interface IHostelService
{
    Task<List<HostelBlueprintDto>> GetHostelsAsync();

    Task<HostelBlueprintDto?> GetHostelBlueprintAsync(int hostelId);

    Task<HostelBlueprintDto> CreateHostelAsync(
        CreateHostelDto dto);

    Task CreateFloorAsync(
        CreateFloorDto dto);

    Task CreateRoomAsync(
        CreateRoomDto dto);

    Task CreateRoomBedAsync(
        CreateRoomBedDto dto);

    Task<bool> UpdateHostelAsync(
        int hostelId,
        UpdateHostelDto dto);

    Task<bool> UpdateFloorAsync(
        int floorId,
        UpdateFloorDto dto);

    Task<bool> UpdateRoomAsync(
        int roomId,
        UpdateRoomDto dto);

    Task<bool> UpdateRoomBedAsync(
        int bedId,
        UpdateRoomBedDto dto);
}