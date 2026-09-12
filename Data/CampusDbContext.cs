// =========================================================
// Auth
// =========================================================
using CampusServicePortal.Modules.Auth.Entities;

// =========================================================
// Complaints
// =========================================================
using CampusServicePortal.Modules.Complaints.Entities;

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

// Canteen class name and Canteen namespace have the same name.
// So we use aliases for these two entities.
using CanteenEntity =
    CampusServicePortal_TicUnicorns.Modules.Canteen.Entities.Canteen;

using CanteenMenuEntity =
    CampusServicePortal_TicUnicorns.Modules.Canteen.Entities.CanteenMenuItem;

// =========================================================
// Certificates
// =========================================================
using CampusServicePortal_TicUnicorns.Modules.Certificates.Entities;

// =========================================================
// Laundry
// =========================================================
using CampusServicePortal_TicUnicorns.Modules.Laundry.Entities;

// =========================================================
// Sports
// =========================================================
using CampusServicePortal_TicUnicorns.Modules.Sports.Entities;

// =========================================================
// Students
// =========================================================
using CampusServicePortal_TicUnicorns.Modules.Students.Entities;

// =========================================================
// EF Core
// =========================================================
using Microsoft.EntityFrameworkCore;


namespace CampusServicePortal_TicUnicorns.Data;

public class CampusDbContext : DbContext
{
    public CampusDbContext(
        DbContextOptions<CampusDbContext> options)
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

    public DbSet<DepartmentStaffAssignment>
        DepartmentStaffAssignments
    { get; set; }

    public DbSet<HostelStaffAssignment>
        HostelStaffAssignments
    { get; set; }

    public DbSet<CanteenStaffAssignment>
        CanteenStaffAssignments
    { get; set; }


    // =========================================================
    // HOSTELS
    // =========================================================

    public DbSet<Hostel> Hostels { get; set; }

    public DbSet<Floor> Floors { get; set; }

    public DbSet<Room> Rooms { get; set; }

    public DbSet<RoomBed> RoomBeds { get; set; }

    public DbSet<HostelApplication>
        HostelApplications
    { get; set; }

    public DbSet<HostelRoomHold>
        HostelRoomHolds
    { get; set; }

    public DbSet<HostelAllocation>
        HostelAllocations
    { get; set; }


    // =========================================================
    // GYM
    // =========================================================

    public DbSet<Gym> Gyms { get; set; }

    public DbSet<GymBooking>
        GymBookings
    { get; set; }

    public DbSet<GymSlot>
        GymSlots
    { get; set; }


    // =========================================================
    // LAUNDRY
    // =========================================================

    public DbSet<LaundryEntities>
        LaundryEntities
    { get; set; }


    // =========================================================
    // LEAVE
    // =========================================================

    public DbSet<LeaveType>
        LeaveTypes
    { get; set; }

    public DbSet<LeaveRequest>
        LeaveRequests
    { get; set; }

    public DbSet<LeaveApproval>
        LeaveApprovals
    { get; set; }


    // =========================================================
    // NOTIFICATIONS
    // =========================================================

    public DbSet<Notification>
        Notifications
    { get; set; }


    // =========================================================
    // SYSTEM SETTINGS
    // =========================================================

    public DbSet<SystemSetting>
        SystemSettings
    { get; set; }


    // =========================================================
    // CANTEENS
    // =========================================================

    // Hostel canteen
    public DbSet<CanteenEntity>
        Canteens
    { get; set; }

    // Individual menu items
    public DbSet<CanteenMenuEntity>
        CanteenMenu
    { get; set; }

    // BB / HB / FB meal plans
    public DbSet<MealPackage>
        MealPackages
    { get; set; }

    // Student meal-plan subscriptions
    public DbSet<MealSubscription>
        MealSubscriptions
    { get; set; }

    public DbSet<MealAbsence>
        MealAbsences
    { get; set; }

    public DbSet<MealUsage>
        MealUsages
    { get; set; }


    // =========================================================
    // CERTIFICATES
    // =========================================================

    public DbSet<CertificatesEntities>
        Certificates
    { get; set; }


    // =========================================================
    // COMPLAINTS
    // =========================================================

    public DbSet<Complaint>
        Complaints
    { get; set; }


    // =========================================================
    // STUDENTS
    // =========================================================

    public DbSet<Student>
        Students
    { get; set; }

    public DbSet<StudentMasterList>
        StudentMasterLists
    { get; set; }


    // =========================================================
    // SPORTS
    // =========================================================

    public DbSet<SportsEvent>
        SportsEvents
    { get; set; }

    public DbSet<SportsEventDepartmentLimit>
        SportsEventDepartmentLimits
    { get; set; }

    public DbSet<SportsRegistration>
        SportsRegistrations
    { get; set; }

    public DbSet<CoachMeeting>
        CoachMeetings
    { get; set; }


    // =========================================================
    // OTP VERIFICATION
    // =========================================================

    public DbSet<OtpVerification>
        OtpVerifications
    { get; set; }


    // =========================================================
    // EF CORE MODEL CONFIGURATION
    // =========================================================

    protected override void OnModelCreating(
        ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(
            typeof(CampusDbContext).Assembly
        );
    }
}