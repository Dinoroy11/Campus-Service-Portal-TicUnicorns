using CampusServicePortal.Modules.SystemSettings.DTOs;
using CampusServicePortal.Modules.SystemSettings.Entities;
using CampusServicePortal.Modules.SystemSettings.Interfaces.Repository;
using CampusServicePortal.Modules.SystemSettings.Interfaces.Service;

namespace CampusServicePortal.Modules.SystemSettings.Services;

public class SystemSettingService : ISystemSettingService
{
    public const string ReservationHoldMinutesKey = "ReservationHoldMinutes";
    public const int DefaultReservationHoldMinutes = 15;

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
        if (string.IsNullOrWhiteSpace(key))
            return null;

        var setting = await _repository.GetSystemSettingByKeyAsync(key.Trim());
        return setting == null ? null : MapToDto(setting);
    }

    public async Task<SystemSettingDto> CreateSystemSettingAsync(
        CreateSystemSettingDto dto)
    {
        var key = NormalizeKey(dto.Key);
        var value = NormalizeValue(dto.Value);

        if (await _repository.KeyExistsAsync(key))
            throw new InvalidOperationException(
                $"A system setting with key '{key}' already exists.");

        ValidateKnownCrossCuttingSetting(key, value);

        var setting = new SystemSetting
        {
            Key = key,
            Value = value,
            Description = dto.Description?.Trim() ?? string.Empty,
            IsActive = true
        };

        var created = await _repository.CreateSystemSettingAsync(setting);
        return MapToDto(created);
    }

    public async Task<SystemSettingDto?> UpdateSystemSettingAsync(
        int settingId,
        UpdateSystemSettingDto dto)
    {
        var setting = await _repository.GetSystemSettingByIdAsync(settingId);
        if (setting == null)
            return null;

        var value = NormalizeValue(dto.Value);
        ValidateKnownCrossCuttingSetting(setting.Key, value);

        setting.Value = value;
        setting.Description = dto.Description?.Trim() ?? string.Empty;

        var updated = await _repository.UpdateSystemSettingAsync(setting);
        return updated == null ? null : MapToDto(updated);
    }

    public async Task<SystemSettingDto?> SetActiveStatusAsync(
        int settingId,
        bool isActive)
    {
        var setting = await _repository.GetSystemSettingByIdAsync(settingId);
        if (setting == null)
            return null;

        setting.IsActive = isActive;
        var updated = await _repository.UpdateSystemSettingAsync(setting);
        return updated == null ? null : MapToDto(updated);
    }

    public async Task<bool> DeactivateSystemSettingAsync(int settingId)
    {
        var setting = await _repository.GetSystemSettingByIdAsync(settingId);
        if (setting == null)
            return false;

        setting.IsActive = false;
        await _repository.UpdateSystemSettingAsync(setting);
        return true;
    }

    public async Task<int> GetReservationHoldMinutesAsync()
    {
        var setting = await _repository
            .GetSystemSettingByKeyAsync(ReservationHoldMinutesKey);

        if (setting == null ||
            !setting.IsActive ||
            !int.TryParse(setting.Value, out var minutes) ||
            minutes < 1 || minutes > 60)
        {
            return DefaultReservationHoldMinutes;
        }

        return minutes;
    }

    public async Task<SystemSettingDto> SetReservationHoldMinutesAsync(int minutes)
    {
        if (minutes < 1 || minutes > 60)
            throw new ArgumentOutOfRangeException(
                nameof(minutes),
                "Reservation hold duration must be between 1 and 60 minutes.");

        var existing = await _repository
            .GetSystemSettingByKeyAsync(ReservationHoldMinutesKey);

        if (existing == null)
        {
            var created = new SystemSetting
            {
                Key = ReservationHoldMinutesKey,
                Value = minutes.ToString(),
                Description = "Default reservation hold duration in minutes for shared booking flows.",
                IsActive = true
            };

            var saved = await _repository.CreateSystemSettingAsync(created);
            return MapToDto(saved);
        }

        existing.Value = minutes.ToString();
        existing.Description =
            "Default reservation hold duration in minutes for shared booking flows.";
        existing.IsActive = true;

        var updated = await _repository.UpdateSystemSettingAsync(existing);
        return MapToDto(updated!);
    }

    private static string NormalizeKey(string key)
    {
        if (string.IsNullOrWhiteSpace(key))
            throw new ArgumentException("System setting key is required.");

        return key.Trim();
    }

    private static string NormalizeValue(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("System setting value is required.");

        return value.Trim();
    }

    private static void ValidateKnownCrossCuttingSetting(string key, string value)
    {
        if (!key.Equals(
                ReservationHoldMinutesKey,
                StringComparison.OrdinalIgnoreCase))
            return;

        if (!int.TryParse(value, out var minutes) || minutes < 1 || minutes > 60)
        {
            throw new ArgumentException(
                "ReservationHoldMinutes must be a whole number between 1 and 60.");
        }
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
