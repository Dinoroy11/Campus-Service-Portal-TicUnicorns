using Microsoft.EntityFrameworkCore;
using CampusServicePortal.Modules.Hostels.Entities;

namespace CampusServicePortal_TicUnicorns.Data;

public class CampusDbContext : DbContext
{
    public CampusDbContext(DbContextOptions<CampusDbContext> options)
        : base(options)
    {
    }

    public DbSet<Hostel> Hostels { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(
            typeof(CampusDbContext).Assembly
        );
    }
}