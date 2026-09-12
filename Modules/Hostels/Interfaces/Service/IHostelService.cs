using CampusServicePortal.Modules.Hostels.DTOs;

namespace CampusServicePortal.Modules.Hostels.Services;

public interface IHostelService
{
    Task<List<HostelBlueprintDto>> GetHostelsAsync();
    Task<HostelBlueprintDto?> GetHostelBlueprintAsync(int hostelId);

    Task<HostelBlueprintDto> CreateHostelAsync(CreateHostelDto dto);
    Task CreateFloorAsync(CreateFloorDto dto);
    Task CreateRoomAsync(CreateRoomDto dto);
    Task CreateRoomBedAsync(CreateRoomBedDto dto);

    Task<bool> UpdateHostelAsync(int hostelId, UpdateHostelDto dto);
    Task<bool> UpdateFloorAsync(int floorId, UpdateFloorDto dto);
    Task<bool> UpdateRoomAsync(int roomId, UpdateRoomDto dto);
    Task<bool> UpdateRoomBedAsync(int bedId, UpdateRoomBedDto dto);

    Task<HostelHoldDto> CreateHoldAsync(int userId, int bedId);
    Task<bool> ReleaseHoldAsync(int userId, int holdId);

    Task<HostelApplicationDto> CreateApplicationAsync(
        int userId,
        CreateHostelApplicationDto dto);

    Task<List<HostelApplicationDto>> GetMyApplicationsAsync(int userId);
    Task<List<HostelApplicationDto>> GetApplicationsAsync();
    Task<HostelApplicationDto?> GetApplicationAsync(int applicationId);
    Task<bool> CancelApplicationAsync(int userId, int applicationId);
    Task<bool> UpdateApplicationStatusAsync(
        int applicationId,
        UpdateHostelApplicationStatusDto dto);

    Task<HostelAllocationDto> AllocateApplicationAsync(
        int applicationId,
        AllocateHostelApplicationDto dto);

    Task<List<HostelAllocationDto>> GetMyAllocationsAsync(int userId);
    Task<List<HostelAllocationDto>> GetAllocationsAsync();
    Task<bool> EndAllocationAsync(int allocationId);

    Task<HostelPaymentDto> GetAllocationPaymentAsync(
        int userId,
        int allocationId);

    Task<HostelPaymentDto> PayAllocationAsync(
        int userId,
        int allocationId,
        PayHostelAllocationDto dto);
}
