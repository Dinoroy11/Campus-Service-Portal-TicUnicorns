using CampusServicePortal.Modules.SystemSettings.Entities;

namespace CampusServicePortal.Modules.SystemSettings.Interfaces.Repository
{
    public interface ISystemSettingRepository
    {
        Task<IEnumerable<SystemSetting>> GetAllSystemSettingsAsync();

        Task<SystemSetting?> GetSystemSettingByIdAsync(int settingId);

        Task<SystemSetting?> GetSystemSettingByKeyAsync(string key);

        Task<SystemSetting> CreateSystemSettingAsync(SystemSetting setting);

        Task<SystemSetting?> UpdateSystemSettingAsync(SystemSetting setting);

        Task<bool> DeleteSystemSettingAsync(int settingId);
    }
}