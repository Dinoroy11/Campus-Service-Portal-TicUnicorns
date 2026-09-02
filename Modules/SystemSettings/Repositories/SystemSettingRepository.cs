using CampusServicePortal.Modules.SystemSettings.Entities;
using CampusServicePortal.Modules.SystemSettings.Interfaces.Repository;

namespace CampusServicePortal.Modules.SystemSettings.Repositories
{
    public class SystemSettingRepository : ISystemSettingRepository
    {
        private readonly List<SystemSetting> _settings = new();

        public Task<IEnumerable<SystemSetting>> GetAllSystemSettingsAsync()
        {
            return Task.FromResult<IEnumerable<SystemSetting>>(_settings);
        }

        public Task<SystemSetting?> GetSystemSettingByIdAsync(int settingId)
        {
            var setting = _settings
                .FirstOrDefault(x => x.SettingId == settingId);

            return Task.FromResult(setting);
        }

        public Task<SystemSetting?> GetSystemSettingByKeyAsync(string key)
        {
            var setting = _settings
                .FirstOrDefault(x =>
                    x.Key.Equals(key, StringComparison.OrdinalIgnoreCase));

            return Task.FromResult(setting);
        }

        public Task<SystemSetting> CreateSystemSettingAsync(
            SystemSetting setting)
        {
            setting.SettingId = _settings.Count + 1;
            setting.IsActive = true;

            _settings.Add(setting);

            return Task.FromResult(setting);
        }

        public Task<SystemSetting?> UpdateSystemSettingAsync(
            SystemSetting setting)
        {
            var existingSetting = _settings
                .FirstOrDefault(x => x.SettingId == setting.SettingId);

            if (existingSetting == null)
            {
                return Task.FromResult<SystemSetting?>(null);
            }

            existingSetting.Key = setting.Key;
            existingSetting.Value = setting.Value;
            existingSetting.Description = setting.Description;
            existingSetting.IsActive = setting.IsActive;

            return Task.FromResult<SystemSetting?>(existingSetting);
        }

        public Task<bool> DeleteSystemSettingAsync(int settingId)
        {
            var setting = _settings
                .FirstOrDefault(x => x.SettingId == settingId);

            if (setting == null)
            {
                return Task.FromResult(false);
            }

            _settings.Remove(setting);

            return Task.FromResult(true);
        }
    }
}