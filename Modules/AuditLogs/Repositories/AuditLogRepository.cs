using CampusServicePortal.Modules.Identity.Entities;
using CampusServicePortal_TicUnicorns.Modules.Identity.Interfaces.Repository;

namespace CampusServicePortal_TicUnicorns.Modules.Identity.Repositories;

public class AuditLogRepository : IAuditLogRepository
{
    public Task AddAsync(AuditLog auditLog)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<AuditLog>> GetAllAsync()
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<AuditLog>> GetByUserIdAsync(int userId)
    {
        throw new NotImplementedException();
    }
}