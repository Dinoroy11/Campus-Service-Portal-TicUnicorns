using CampusServicePortal.Modules.Identity.DTOs;

namespace CampusServicePortal_TicUnicorns.Modules.Identity.Interfaces.Service;

public interface IAuditLogService
{
    Task<AuditLogDto> CreateAsync(AuditLogDto dto);

    Task<IEnumerable<AuditLogDto>> GetAllAsync();

    Task<IEnumerable<AuditLogDto>> GetByUserIdAsync(int userId);
}