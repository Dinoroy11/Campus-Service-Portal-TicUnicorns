using CampusServicePortal.Modules.Identity.Entities;

namespace CampusServicePortal_TicUnicorns.Modules.Identity.Interfaces.Repository;

public interface IAuditLogRepository
{
    Task AddAsync(AuditLog auditLog);

    Task<IEnumerable<AuditLog>> GetAllAsync();

    Task<IEnumerable<AuditLog>> GetByUserIdAsync(int userId);
}
