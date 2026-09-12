using CampusServicePortal.Modules.SystemSettings.DTOs;

namespace CampusServicePortal.Modules.SystemSettings.Interfaces.Service;

public interface ISystemSettingService
{
    Task<IEnumerable<SystemSettingDto>> GetAllSystemSettingsAsync();
    Task<SystemSettingDto?> GetSystemSettingByIdAsync(int settingId);
    Task<SystemSettingDto?> GetSystemSettingByKeyAsync(string key);
    Task<SystemSettingDto> CreateSystemSettingAsync(CreateSystemSettingDto dto);
    Task<SystemSettingDto?> UpdateSystemSettingAsync(int settingId, UpdateSystemSettingDto dto);
    Task<SystemSettingDto?> SetActiveStatusAsync(int settingId, bool isActive);
    Task<bool> DeactivateSystemSettingAsync(int settingId);

    // Shared cross-cutting setting used as the default reservation hold timeout.
    Task<int> GetReservationHoldMinutesAsync();
    Task<SystemSettingDto> SetReservationHoldMinutesAsync(int minutes);
}
