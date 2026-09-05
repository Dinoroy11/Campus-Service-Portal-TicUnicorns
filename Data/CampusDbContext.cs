using CampusServicePortal.Modules.Hostels.Entities;
using CampusServicePortal_TicUnicorns.Modules.AuditLogs.Entities;
using CampusServicePortal_TicUnicorns.Modules.Canteen.Entities;
using CampusServicePortal_TicUnicorns.Modules.Certificates.Entities;
using CampusServicePortal_TicUnicorns.Modules.Complaints.Entities;
using CampusServicePortal_TicUnicorns.Modules.Events.Entities;
using CampusServicePortal_TicUnicorns.Modules.Fees.Entities;
using CampusServicePortal_TicUnicorns.Modules.Gym.Entities;
using CampusServicePortal_TicUnicorns.Modules.Hostels.Entities;
using CampusServicePortal_TicUnicorns.Modules.Identity.Entities;
using CampusServicePortal_TicUnicorns.Modules.Labs.Entities;
using CampusServicePortal_TicUnicorns.Modules.Laundry.Entities;
using CampusServicePortal_TicUnicorns.Modules.Leave.Entities;
using CampusServicePortal_TicUnicorns.Modules.Notifications.Entities;
using CampusServicePortal_TicUnicorns.Modules.Sports.Entities;
using CampusServicePortal_TicUnicorns.Modules.Students.Entities;
using CampusServicePortal_TicUnicorns.Modules.SystemSettings.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Data;
using System.Security;
using System.Security.AccessControl;
using System.Security.Cryptography.X509Certificates;

namespace CampusServicePortal_TicUnicorns.Data;

public class CampusDbContext : DbContext
{
    public CampusDbContext(DbContextOptions<CampusDbContext> options)
        : base(options)
    {
    }

    // =========================================================
    // Identity & Access
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
    // Master Data
    // =========================================================

    public DbSet<University> Universities { get; set; }
    public DbSet<Faculty> Faculties { get; set; }
    public DbSet<Department> Departments { get; set; }

    public DbSet<Hostel> Hostels { get; set; }
    public DbSet<Floor> Floors { get; set; }
    public DbSet<Room> Rooms { get; set; }
    public DbSet<RoomBed> RoomBeds { get; set; }

    public DbSet<Canteen> Canteens { get; set; }


    // =========================================================
    // Students
    // =========================================================

    public DbSet<StudentMasterList> StudentMasterLists { get; set; }
    public DbSet<Student> Students { get; set; }
    public DbSet<StudentAcademic> StudentAcademics { get; set; }
    public DbSet<StudentDocument> StudentDocuments { get; set; }
    public DbSet<StudentEmergencyContact> StudentEmergencyContacts { get; set; }


    // =========================================================
    // Hostels
    // =========================================================

    public DbSet<HostelApplication> HostelApplications { get; set; }
    public DbSet<HostelRoomHold> HostelRoomHolds { get; set; }
    public DbSet<HostelAllocation> HostelAllocations { get; set; }


    // =========================================================
    // Labs
    // =========================================================

    public DbSet<Lab> Labs { get; set; }
    public DbSet<LabSeat> LabSeats { get; set; }
    public DbSet<LabTimeSlot> LabTimeSlots { get; set; }
    public DbSet<LabBooking> LabBookings { get; set; }


    // =========================================================
    // Events
    // =========================================================

    public DbSet<Venue> Venues { get; set; }
    public DbSet<Event> Events { get; set; }
    public DbSet<EventSeat> EventSeats { get; set; }
    public DbSet<EventRegistration> EventRegistrations { get; set; }
    public DbSet<EventPayment> EventPayments { get; set; }


    // =========================================================
    // Fees
    // =========================================================

    public DbSet<FeeType> FeeTypes { get; set; }
    public DbSet<StudentFee> StudentFees { get; set; }
    public DbSet<FeePayment> FeePayments { get; set; }
    public DbSet<RefundRequest> RefundRequests { get; set; }


    // =========================================================
    // Certificates
    // =========================================================

    public DbSet<CertificateType> CertificateTypes { get; set; }
    public DbSet<CertificateRequest> CertificateRequests { get; set; }
    public DbSet<CertificateCollection> CertificateCollections { get; set; }


    // =========================================================
    // Complaints
    // =========================================================

    public DbSet<ComplaintCategory> ComplaintCategories { get; set; }
    public DbSet<Complaint> Complaints { get; set; }
    public DbSet<ComplaintStatusHistory> ComplaintStatusHistories { get; set; }


    // =========================================================
    // Canteen
    // =========================================================

    public DbSet<CanteenMenu> CanteenMenus { get; set; }
    public DbSet<MealPlan> MealPlans { get; set; }
    public DbSet<MealSubscription> MealSubscriptions { get; set; }


    // =========================================================
    // Laundry
    // =========================================================

    public DbSet<LaundryService> LaundryServices { get; set; }
    public DbSet<LaundrySchedule> LaundrySchedules { get; set; }
    public DbSet<LaundryTimeSlot> LaundryTimeSlots { get; set; }
    public DbSet<LaundryRequest> LaundryRequests { get; set; }


    // =========================================================
    // Sports
    // =========================================================

    public DbSet<SportsEvent> SportsEvents { get; set; }
    public DbSet<SportsEventDepartmentLimit> SportsEventDepartmentLimits { get; set; }
    public DbSet<SportsRegistration> SportsRegistrations { get; set; }
    public DbSet<CoachMeeting> CoachMeetings { get; set; }


    // =========================================================
    // Gym
    // =========================================================

    public DbSet<GymFacility> GymFacilities { get; set; }
    public DbSet<GymTimeSlot> GymTimeSlots { get; set; }
    public DbSet<GymBooking> GymBookings { get; set; }


    // =========================================================
    // Leave
    // =========================================================

    public DbSet<LeaveType> LeaveTypes { get; set; }
    public DbSet<LeaveRequest> LeaveRequests { get; set; }
    public DbSet<LeaveApproval> LeaveApprovals { get; set; }


    // =========================================================
    // Notifications
    // =========================================================

    public DbSet<Notification> Notifications { get; set; }


    // =========================================================
    // System Settings
    // =========================================================

    public DbSet<SystemSetting> SystemSettings { get; set; }


    // =========================================================
    // EF Core Configurations
    // =========================================================

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(
            typeof(CampusDbContext).Assembly
        );
    }
}