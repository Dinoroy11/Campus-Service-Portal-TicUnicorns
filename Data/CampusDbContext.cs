// =========================================================
// Gym
// =========================================================
using CampusServicePortal.Modules.Gym.Entities;
// =========================================================
// Hostels
// =========================================================
using CampusServicePortal.Modules.Hostels.Entities;
// =========================================================
// Identity
// =========================================================
using CampusServicePortal.Modules.Identity.Entities;
// =========================================================
// Leave
// =========================================================
using CampusServicePortal.Modules.Leave.Entities;
// =========================================================
// Notifications
// =========================================================
using CampusServicePortal.Modules.Notifications.Entities;
// =========================================================
// System Settings
// =========================================================
using CampusServicePortal.Modules.SystemSettings.Entities;


// =========================================================
// Canteens
// =========================================================
using CampusServicePortal_TicUnicorns.Modules.Canteen.Entities;
// =========================================================
// Certificates
// =========================================================
using CampusServicePortal_TicUnicorns.Modules.Certificates.Entities;
// =========================================================
// Complaints
// =========================================================

using CampusServicePortal.Modules.Complaints.Entities;
// =========================================================
// Laundry
// =========================================================
using CampusServicePortal_TicUnicorns.Modules.Laundry.Entities;
using Microsoft.EntityFrameworkCore;


namespace CampusServicePortal_TicUnicorns.Data;

public class CampusDbContext : DbContext
{
    public CampusDbContext(DbContextOptions<CampusDbContext> options)
        : base(options)
    {
    }


    // =========================================================
    // IDENTITY & ACCESS
    // =========================================================

    public DbSet<User> Users { get; set; }

    public DbSet<UserRole> UserRoles { get; set; }

    public DbSet<Role> Roles { get; set; }

    public DbSet<RolePermission> RolePermissions { get; set; }

    public DbSet<Permission> Permissions { get; set; }

    public DbSet<AuditLog> AuditLogs { get; set; }

    public DbSet<DepartmentStaffAssignment> DepartmentStaffAssignments { get; set; }

    public DbSet<HostelStaffAssignment> HostelStaffAssignments { get; set; }

    public DbSet<CanteenStaffAssignment> CanteenStaffAssignments { get; set; }


    // =========================================================
    // HOSTELS
    // =========================================================

    public DbSet<Hostel> Hostels { get; set; }

    public DbSet<Floor> Floors { get; set; }

    public DbSet<Room> Rooms { get; set; }

    public DbSet<RoomBed> RoomBeds { get; set; }

    public DbSet<HostelApplication> HostelApplications { get; set; }

    public DbSet<HostelRoomHold> HostelRoomHolds { get; set; }

    public DbSet<HostelAllocation> HostelAllocations { get; set; }


    // =========================================================
    // GYM
    // =========================================================

    public DbSet<Gym> Gyms { get; set; }

    public DbSet<GymBooking> GymBookings { get; set; }

    public DbSet<GymSlot> GymSlots { get; set; }


    // =========================================================
    // LAUNDRY
    // =========================================================

    public DbSet<LaundryEntities> LaundryEntities { get; set; }


    // =========================================================
    // LEAVE
    // =========================================================

    public DbSet<Leave> Leaves { get; set; }


    // =========================================================
    // NOTIFICATIONS
    // =========================================================

    public DbSet<Notification> Notifications { get; set; }


    // =========================================================
    // SYSTEM SETTINGS
    // =========================================================

    public DbSet<SystemSetting> SystemSettings { get; set; }



    // =========================================================
    // CANTEENS
    // =========================================================

    public DbSet<MealPackage> MealPackages { get; set; }
    public DbSet<MealSubscription> MealSubscriptions { get; set; }
    public DbSet<MealAbsence> MealAbsences { get; set; }
    public DbSet<MealUsage> MealUsages { get; set; }


    // =========================================================
    // CERTIFICATES
    // =========================================================

    public DbSet<CertificatesEntities> Certificates { get; set; }


    // =========================================================
    // COMPLAINTS
    // =========================================================
    public DbSet<Complaint> Complaints { get; set; }

    // =========================================================
    // EF CORE MODEL CONFIGURATION
    // =========================================================

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(
            typeof(CampusDbContext).Assembly
        );
    }
}