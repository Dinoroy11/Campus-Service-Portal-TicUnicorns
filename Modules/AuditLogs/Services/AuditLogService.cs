using CampusServicePortal.Modules.Identity.DTOs;
using CampusServicePortal.Modules.Identity.Entities;
using CampusServicePortal_TicUnicorns.Modules.Identity.Interfaces.Repository;
using CampusServicePortal_TicUnicorns.Modules.Identity.Interfaces.Service;

namespace CampusServicePortal_TicUnicorns.Modules.Identity.Services;

public class AuditLogService : IAuditLogService
{
    private readonly IAuditLogRepository _auditLogRepository;

    public AuditLogService(IAuditLogRepository auditLogRepository)
    {
        _auditLogRepository = auditLogRepository;
    }

    public async Task<AuditLogDto> CreateAsync(AuditLogDto dto)
    {
        ValidateAuditLog(dto);

        var auditLog = new AuditLog
        {
            UserId = dto.UserId,
            EntityType = dto.EntityType.Trim(),
            EntityId = dto.EntityId,
            Action = dto.Action.Trim(),
            OldValue = NormalizeOptionalValue(dto.OldValue),
            NewValue = NormalizeOptionalValue(dto.NewValue),
            CreatedAt = DateTime.UtcNow
        };

        await _auditLogRepository.AddAsync(auditLog);

        return MapToDto(auditLog);
    }

    public Task<AuditLogDto> LogAsync(
        int userId,
        string entityType,
        int entityId,
        string action,
        string? oldValue = null,
        string? newValue = null)
    {
        return CreateAsync(new AuditLogDto
        {
            UserId = userId,
            EntityType = entityType,
            EntityId = entityId,
            Action = action,
            OldValue = oldValue,
            NewValue = newValue
        });
    }

    public async Task<IEnumerable<AuditLogDto>> GetAllAsync()
    {
        var auditLogs = await _auditLogRepository.GetAllAsync();
        return auditLogs.Select(MapToDto);
    }

    public async Task<IEnumerable<AuditLogDto>> GetByUserIdAsync(int userId)
    {
        if (userId <= 0)
        {
            throw new ArgumentException(
                "User ID must be greater than zero.");
        }

        var auditLogs = await _auditLogRepository
            .GetByUserIdAsync(userId);

        return auditLogs.Select(MapToDto);
    }

    private static void ValidateAuditLog(AuditLogDto dto)
    {
        if (dto == null)
        {
            throw new ArgumentNullException(nameof(dto));
        }

        if (dto.UserId <= 0)
        {
            throw new ArgumentException(
                "User ID must be greater than zero.");
        }

        if (string.IsNullOrWhiteSpace(dto.EntityType))
        {
            throw new ArgumentException(
                "Entity type is required.");
        }

        if (dto.EntityType.Trim().Length > 100)
        {
            throw new ArgumentException(
                "Entity type cannot exceed 100 characters.");
        }

        if (dto.EntityId <= 0)
        {
            throw new ArgumentException(
                "Entity ID must be greater than zero.");
        }

        if (string.IsNullOrWhiteSpace(dto.Action))
        {
            throw new ArgumentException(
                "Audit action is required.");
        }

        if (dto.Action.Trim().Length > 100)
        {
            throw new ArgumentException(
                "Audit action cannot exceed 100 characters.");
        }

        if (ContainsSensitiveValue(dto.OldValue) ||
            ContainsSensitiveValue(dto.NewValue))
        {
            throw new ArgumentException(
                "Sensitive values must not be stored in audit logs.");
        }
    }

    private static string? NormalizeOptionalValue(string? value)
    {
        return string.IsNullOrWhiteSpace(value)
            ? null
            : value.Trim();
    }

    private static bool ContainsSensitiveValue(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return false;
        }

        var normalizedValue = value.ToLowerInvariant();

        var sensitiveKeywords = new[]
        {
            "password",
            "passwordhash",
            "otp",
            "token",
            "secret",
            "access_token",
            "refresh_token",
            "accesstoken",
            "refreshtoken"
        };

        return sensitiveKeywords.Any(normalizedValue.Contains);
    }

    private static AuditLogDto MapToDto(AuditLog auditLog)
    {
        return new AuditLogDto
        {
            AuditLogId = auditLog.AuditLogId,
            UserId = auditLog.UserId,
            EntityType = auditLog.EntityType,
            EntityId = auditLog.EntityId,
            Action = auditLog.Action,
            OldValue = auditLog.OldValue,
            NewValue = auditLog.NewValue,
            CreatedAt = auditLog.CreatedAt
        };
    }
}
