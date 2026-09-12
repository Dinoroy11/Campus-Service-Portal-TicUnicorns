using CampusServicePortal.Modules.Identity.DTOs;

namespace CampusServicePortal_TicUnicorns.Modules.Identity.Interfaces.Service;

public interface IAuditLogService
{
    // Internal backend write API. Do not expose this from a public controller.
    Task<AuditLogDto> CreateAsync(AuditLogDto dto);

    // Convenience method for module services.
    Task<AuditLogDto> LogAsync(
        int userId,
        string entityType,
        int entityId,
        string action,
        string? oldValue = null,
        string? newValue = null);

    Task<IEnumerable<AuditLogDto>> GetAllAsync();

    Task<IEnumerable<AuditLogDto>> GetByUserIdAsync(int userId);
}
