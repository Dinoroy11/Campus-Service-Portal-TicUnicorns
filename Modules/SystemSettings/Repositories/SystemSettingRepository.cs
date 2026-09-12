using CampusServicePortal.Modules.SystemSettings.Entities;
using CampusServicePortal.Modules.SystemSettings.Interfaces.Repository;
using CampusServicePortal_TicUnicorns.Data;
using Microsoft.EntityFrameworkCore;

namespace CampusServicePortal.Modules.SystemSettings.Repositories;

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
            .OrderBy(s => s.Key)
            .ToListAsync();
    }

    public async Task<SystemSetting?> GetSystemSettingByIdAsync(int settingId)
    {
        return await _context.SystemSettings
            .FirstOrDefaultAsync(s => s.SettingId == settingId);
    }

    public async Task<SystemSetting?> GetSystemSettingByKeyAsync(string key)
    {
        var normalizedKey = key.Trim();

        return await _context.SystemSettings
            .FirstOrDefaultAsync(s => s.Key == normalizedKey);
    }

    public async Task<bool> KeyExistsAsync(
        string key,
        int? excludeSettingId = null)
    {
        var normalizedKey = key.Trim();

        return await _context.SystemSettings.AnyAsync(s =>
            s.Key == normalizedKey &&
            (!excludeSettingId.HasValue || s.SettingId != excludeSettingId.Value));
    }

    public async Task<SystemSetting> CreateSystemSettingAsync(SystemSetting setting)
    {
        await _context.SystemSettings.AddAsync(setting);
        await _context.SaveChangesAsync();
        return setting;
    }

    public async Task<SystemSetting?> UpdateSystemSettingAsync(SystemSetting setting)
    {
        var existing = await _context.SystemSettings
            .FirstOrDefaultAsync(s => s.SettingId == setting.SettingId);

        if (existing == null)
            return null;

        existing.Key = setting.Key;
        existing.Value = setting.Value;
        existing.Description = setting.Description;
        existing.IsActive = setting.IsActive;

        await _context.SaveChangesAsync();
        return existing;
    }
}
