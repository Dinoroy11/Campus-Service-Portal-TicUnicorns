using CampusServicePortal.Modules.SystemSettings.DTOs;

namespace CampusServicePortal.Modules.SystemSettings.Interfaces.Service
{
    public interface ISystemSettingService
    {
        Task<IEnumerable<SystemSettingDto>> GetAllSystemSettingsAsync();

        Task<SystemSettingDto?> GetSystemSettingByIdAsync(int settingId);

        Task<SystemSettingDto?> GetSystemSettingByKeyAsync(string key);

        Task<SystemSettingDto> CreateSystemSettingAsync(
            CreateSystemSettingDto settingDto);

        Task<SystemSettingDto?> UpdateSystemSettingAsync(
            int settingId,
            CreateSystemSettingDto settingDto);

        Task<bool> DeleteSystemSettingAsync(int settingId);
    }
}