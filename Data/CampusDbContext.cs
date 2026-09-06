using Microsoft.EntityFrameworkCore;

namespace CampusServicePortal_TicUnicorns.Data;

public class CampusDbContext : DbContext
{
    public CampusDbContext(DbContextOptions<CampusDbContext> options)
        : base(options)
    {
    }
}