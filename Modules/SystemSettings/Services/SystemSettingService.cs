using CampusServicePortal.Modules.SystemSettings.DTOs;
using CampusServicePortal.Modules.SystemSettings.Entities;
using CampusServicePortal.Modules.SystemSettings.Interfaces.Repository;
using CampusServicePortal.Modules.SystemSettings.Interfaces.Service;

namespace CampusServicePortal.Modules.SystemSettings.Services
{
    public class SystemSettingService : ISystemSettingService
    {
        private readonly ISystemSettingRepository _repository;

        public SystemSettingService(ISystemSettingRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<SystemSettingDto>> GetAllSystemSettingsAsync()
        {
            var settings = await _repository.GetAllSystemSettingsAsync();

            return settings.Select(MapToDto);
        }

        public async Task<SystemSettingDto?> GetSystemSettingByIdAsync(int settingId)
        {
            var setting = await _repository.GetSystemSettingByIdAsync(settingId);

            return setting == null ? null : MapToDto(setting);
        }

        public async Task<SystemSettingDto?> GetSystemSettingByKeyAsync(string key)
        {
            var setting = await _repository.GetSystemSettingByKeyAsync(key);

            return setting == null ? null : MapToDto(setting);
        }

        public async Task<SystemSettingDto> CreateSystemSettingAsync(
            CreateSystemSettingDto settingDto)
        {
            var setting = new SystemSetting
            {
                Key = settingDto.Key,
                Value = settingDto.Value,
                Description = settingDto.Description,
                IsActive = true
            };

            var createdSetting =
                await _repository.CreateSystemSettingAsync(setting);

            return MapToDto(createdSetting);
        }

        public async Task<SystemSettingDto?> UpdateSystemSettingAsync(
            int settingId,
            CreateSystemSettingDto settingDto)
        {
            var setting = await _repository.GetSystemSettingByIdAsync(settingId);

            if (setting == null)
            {
                return null;
            }

            setting.Key = settingDto.Key;
            setting.Value = settingDto.Value;
            setting.Description = settingDto.Description;

            var updatedSetting =
                await _repository.UpdateSystemSettingAsync(setting);

            return updatedSetting == null ? null : MapToDto(updatedSetting);
        }

        public async Task<bool> DeleteSystemSettingAsync(int settingId)
        {
            return await _repository.DeleteSystemSettingAsync(settingId);
        }

        private static SystemSettingDto MapToDto(SystemSetting setting)
        {
            return new SystemSettingDto
            {
                SettingId = setting.SettingId,
                Key = setting.Key,
                Value = setting.Value,
                Description = setting.Description,
                IsActive = setting.IsActive
            };
        }
    }
}