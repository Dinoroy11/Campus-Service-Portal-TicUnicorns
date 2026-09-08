using CampusServicePortal.Modules.SystemSettings.Entities;
using CampusServicePortal.Modules.SystemSettings.Interfaces.Repository;
using CampusServicePortal_TicUnicorns.Data;
using Microsoft.EntityFrameworkCore;

namespace CampusServicePortal.Modules.SystemSettings.Repositories
{
    public class SystemSettingRepository : ISystemSettingRepository
    {
        private readonly CampusDbContext _context;

        public SystemSettingRepository(CampusDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<SystemSetting>> GetAllSystemSettingsAsync()
        {
            return await _context.SystemSettings
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<SystemSetting?> GetSystemSettingByIdAsync(int settingId)
        {
            return await _context.SystemSettings
                .FirstOrDefaultAsync(s => s.SettingId == settingId);
        }

        public async Task<SystemSetting?> GetSystemSettingByKeyAsync(string key)
        {
            return await _context.SystemSettings
                .FirstOrDefaultAsync(s => s.Key == key);
        }

        public async Task<SystemSetting> CreateSystemSettingAsync(
            SystemSetting setting)
        {
            setting.IsActive = true;

            await _context.SystemSettings.AddAsync(setting);
            await _context.SaveChangesAsync();

            return setting;
        }

        public async Task<SystemSetting?> UpdateSystemSettingAsync(
            SystemSetting setting)
        {
            var existingSetting = await _context.SystemSettings
                .FirstOrDefaultAsync(s => s.SettingId == setting.SettingId);

            if (existingSetting == null)
            {
                return null;
            }

            existingSetting.Key = setting.Key;
            existingSetting.Value = setting.Value;
            existingSetting.Description = setting.Description;
            existingSetting.IsActive = setting.IsActive;

            await _context.SaveChangesAsync();

            return existingSetting;
        }

        public async Task<bool> DeleteSystemSettingAsync(int settingId)
        {
            var setting = await _context.SystemSettings
                .FirstOrDefaultAsync(s => s.SettingId == settingId);

            if (setting == null)
            {
                return false;
            }

            _context.SystemSettings.Remove(setting);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}