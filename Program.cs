// =========================================================
// Complaints
// =========================================================
using CampusServicePortal.Modules.Complaints.Interfaces.Repository;
using CampusServicePortal.Modules.Complaints.Interfaces.Service;
using CampusServicePortal.Modules.Complaints.Repositories;
using CampusServicePortal.Modules.Complaints.Services;
using CampusServicePortal.Modules.Events.Repositories;
using CampusServicePortal.Modules.Events.Services;
// =========================================================
// Fees
// =========================================================
using CampusServicePortal.Modules.Fees.Interfaces.Repository;
using CampusServicePortal.Modules.Fees.Interfaces.Service;
using CampusServicePortal.Modules.Fees.Repositories;
using CampusServicePortal.Modules.Fees.Services;
// =========================================================
// Gym
// =========================================================
using CampusServicePortal.Modules.Gym.Interfaces.Repository;
using CampusServicePortal.Modules.Gym.Interfaces.Service;
using CampusServicePortal.Modules.Gym.Repositories;
using CampusServicePortal.Modules.Gym.Services;
// =========================================================
// Hostels
// =========================================================
using CampusServicePortal.Modules.Hostels.Repositories;
using CampusServicePortal.Modules.Hostels.Services;
// =========================================================
// Identity
// =========================================================
using CampusServicePortal.Modules.Identity.Interfaces.Repository;
using CampusServicePortal.Modules.Identity.Interfaces.Service;
// =========================================================
// Labs
// =========================================================
using CampusServicePortal.Modules.Labs.Interfaces.Repository;
using CampusServicePortal.Modules.Labs.Interfaces.Service;
using CampusServicePortal.Modules.Labs.Repositories;
using CampusServicePortal.Modules.Labs.Services;
// =========================================================
// Leave
// =========================================================
using CampusServicePortal.Modules.Leave.Interfaces.Repository;
using CampusServicePortal.Modules.Leave.Interfaces.Service;
using CampusServicePortal.Modules.Leave.Repositories;
using CampusServicePortal.Modules.Leave.Services;
// =========================================================
// Notifications
// =========================================================
using CampusServicePortal.Modules.Notifications.Interfaces.Repository;
using CampusServicePortal.Modules.Notifications.Interfaces.Service;
using CampusServicePortal.Modules.Notifications.Repositories;
using CampusServicePortal.Modules.Notifications.Services;
// =========================================================
// System Settings
// =========================================================
using CampusServicePortal.Modules.SystemSettings.Interfaces.Repository;
using CampusServicePortal.Modules.SystemSettings.Interfaces.Service;
using CampusServicePortal.Modules.SystemSettings.Repositories;
using CampusServicePortal.Modules.SystemSettings.Services;
// =========================================================
// Data
// =========================================================
using CampusServicePortal_TicUnicorns.Data;
using CampusServicePortal_TicUnicorns.Data.SeedData;
// =========================================================
// Auth
// =========================================================
using CampusServicePortal_TicUnicorns.Modules.Auth.Interfaces.Repository;
using CampusServicePortal_TicUnicorns.Modules.Auth.Interfaces.Service;
using CampusServicePortal_TicUnicorns.Modules.Auth.Repositories;
using CampusServicePortal_TicUnicorns.Modules.Auth.Services;

// =========================================================
// Canteen
// =========================================================
using CampusServicePortal_TicUnicorns.Modules.Canteen.Repositories;
using CampusServicePortal_TicUnicorns.Modules.Canteen.Repositories.Interfaces;
using CampusServicePortal_TicUnicorns.Modules.Canteen.Services;
using CampusServicePortal_TicUnicorns.Modules.Canteen.Services.Interfaces;
// =========================================================
// Certificates
// =========================================================
using CampusServicePortal_TicUnicorns.Modules.Certificates.Interfaces.Repository;
using CampusServicePortal_TicUnicorns.Modules.Certificates.Interfaces.Service;
using CampusServicePortal_TicUnicorns.Modules.Certificates.Repositories;
using CampusServicePortal_TicUnicorns.Modules.Certificates.Services;
// =========================================================
// Events
// =========================================================
using CampusServicePortal_TicUnicorns.Modules.Events.Interfaces.Service;
// =========================================================
// Identity / Audit Logs
// =========================================================
using CampusServicePortal_TicUnicorns.Modules.Identity.Interfaces.Repository;
using CampusServicePortal_TicUnicorns.Modules.Identity.Interfaces.Service;
using CampusServicePortal_TicUnicorns.Modules.Identity.Repositories;
using CampusServicePortal_TicUnicorns.Modules.Identity.Services;
// =========================================================
// Laundry
// =========================================================
using CampusServicePortal_TicUnicorns.Modules.Laundry.Interfaces.Repository;
using CampusServicePortal_TicUnicorns.Modules.Laundry.Interfaces.Service;
using CampusServicePortal_TicUnicorns.Modules.Laundry.Repositories;
using CampusServicePortal_TicUnicorns.Modules.Laundry.Services;
// =========================================================
// Sports
// =========================================================
using CampusServicePortal_TicUnicorns.Modules.Sports.Interfaces.Repository;
using CampusServicePortal_TicUnicorns.Modules.Sports.Interfaces.Service;
using CampusServicePortal_TicUnicorns.Modules.Sports.Repositories;
using CampusServicePortal_TicUnicorns.Modules.Sports.Services;
// =========================================================
// Students
// =========================================================
using CampusServicePortal_TicUnicorns.Modules.Students.Interfaces.Repository;
using CampusServicePortal_TicUnicorns.Modules.Students.Interfaces.Service;
using CampusServicePortal_TicUnicorns.Modules.Students.Repositories;
using CampusServicePortal_TicUnicorns.Modules.Students.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
 
var builder = WebApplication.CreateBuilder(args);

// =========================================================
// Database
// =========================================================

builder.Services.AddDbContext<CampusDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")
        ?? "Server=(localdb)\\MSSQLLocalDB;Database=CampusServicePortalDb;Trusted_Connection=True;TrustServerCertificate=True;"
    )
);

// =========================================================
// Authentication / JWT
// =========================================================

var jwtKey = builder.Configuration["Jwt:Key"];

if (string.IsNullOrWhiteSpace(jwtKey))
{
    throw new InvalidOperationException(
        "JWT signing key is not configured.");
}

builder.Services
    .AddAuthentication("Bearer")
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,

            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtKey)),

            ValidateIssuer = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],

            ValidateAudience = true,
            ValidAudience = builder.Configuration["Jwt:Audience"],

            ValidateLifetime = true,

            ClockSkew = TimeSpan.Zero
        };
    });

builder.Services.AddAuthorization();

// =========================================================
// Controllers
// =========================================================

builder.Services.AddControllers();

// =========================================================
// Audit Logs
// =========================================================

builder.Services.AddScoped<IAuditLogRepository, AuditLogRepository>();
builder.Services.AddScoped<IAuditLogService, AuditLogService>();

// =========================================================
// Auth
// =========================================================

builder.Services.AddScoped<IAuthRepository, AuthRepository>();
builder.Services.AddScoped<IAuthService, AuthService>();

// =========================================================
// Canteen
// =========================================================

builder.Services.AddScoped<ICanteenRepository, CanteenRepository>();
builder.Services.AddScoped<ICanteenService, CanteenService>();

// =========================================================
// Certificates
// =========================================================

builder.Services.AddScoped<ICertificatesRepository, CertificatesRepository>();
builder.Services.AddScoped<ICertificatesService, CertificatesService>();

// =========================================================
// Complaints
// =========================================================

builder.Services.AddScoped<IComplaintCategoryRepository, ComplaintCategoryRepository>();
builder.Services.AddScoped<IComplaintRepository, ComplaintRepository>();
builder.Services.AddScoped<IComplaintStatusHistoryRepository, ComplaintStatusHistoryRepository>();
builder.Services.AddScoped<IComplaintsService, ComplaintsService>();

// =========================================================
// Events
// =========================================================

builder.Services.AddScoped<IEventRepository, EventRepository>();
builder.Services.AddScoped<IEventPaymentRepository, EventPaymentRepository>();
builder.Services.AddScoped<IEventRegistrationRepository, EventRegistrationRepository>();
builder.Services.AddScoped<IEventSeatRepository, EventSeatRepository>();
builder.Services.AddScoped<IVenueRepository, VenueRepository>();

builder.Services.AddScoped<IEventService, EventService>();
builder.Services.AddScoped<IEventPaymentService, EventPaymentService>();
builder.Services.AddScoped<IEventRegistrationService, EventRegistrationService>();
builder.Services.AddScoped<IEventSeatService, EventSeatService>();
builder.Services.AddScoped<IVenueService, VenueService>();

// =========================================================
// Fees
// =========================================================

builder.Services.AddScoped<IFeePaymentRepository, FeePaymentRepository>();
builder.Services.AddScoped<IFeeTypeRepository, FeeTypeRepository>();
builder.Services.AddScoped<IRefundRequestRepository, RefundRequestRepository>();
builder.Services.AddScoped<IStudentFeeRepository, StudentFeeRepository>();

builder.Services.AddScoped<IFeesService, FeesService>();

// =========================================================
// Gym
// =========================================================

builder.Services.AddScoped<IGymRepository, GymRepository>();
builder.Services.AddScoped<IGymService, GymService>();

// =========================================================
// Hostels
// =========================================================

builder.Services.AddScoped<IHostelRepository, HostelRepository>();
builder.Services.AddScoped<IHostelService, HostelService>();

// =========================================================
// Labs
// =========================================================

builder.Services.AddScoped<ILabRepository, LabRepository>();
builder.Services.AddScoped<ILabSeatRepository, LabSeatRepository>();
builder.Services.AddScoped<ILabTimeSlotRepository, LabTimeSlotRepository>();
builder.Services.AddScoped<ILabBookingRepository, LabBookingRepository>();

builder.Services.AddScoped<ILabService, LabService>();

// =========================================================
// Laundry
// =========================================================

builder.Services.AddScoped<ILaundryRepository, LaundryRepository>();
builder.Services.AddScoped<ILaundryService, LaundryService>();

// =========================================================
// Leave
// =========================================================

builder.Services.AddScoped<ILeaveRepository, LeaveRepository>();
builder.Services.AddScoped<ILeaveService, LeaveService>();

// =========================================================
// Notifications
// =========================================================

builder.Services.AddScoped<INotificationRepository, NotificationRepository>();
builder.Services.AddScoped<INotificationService, NotificationService>();

// =========================================================
// Identity
// =========================================================

builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IRoleRepository, RoleRepository>();
builder.Services.AddScoped<IPermissionRepository, PermissionRepository>();

builder.Services.AddScoped<IUserRoleRepository, UserRoleRepository>();
builder.Services.AddScoped<IRolePermissionRepository, RolePermissionRepository>();
builder.Services.AddScoped<IDepartmentStaffAssignmentRepository, DepartmentStaffAssignmentRepository>();
builder.Services.AddScoped<IHostelStaffAssignmentRepository, HostelStaffAssignmentRepository>();
builder.Services.AddScoped<ICanteenStaffAssignmentRepository, CanteenStaffAssignmentRepository>();

builder.Services.AddScoped<IIdentityService, IdentityService>();

// =========================================================
// Sports
// =========================================================

builder.Services.AddScoped<ICoachMeetingRepository, CoachMeetingRepository>();
builder.Services.AddScoped<ISportsEventRepository, SportsEventRepository>();
builder.Services.AddScoped<ISportsEventDepartmentLimitRepository, SportsEventDepartmentLimitRepository>();
builder.Services.AddScoped<ISportsRegistrationRepository, SportsRegistrationRepository>();

builder.Services.AddScoped<ICoachMeetingService, CoachMeetingService>();
builder.Services.AddScoped<ISportsEventService, SportsEventService>();
builder.Services.AddScoped<ISportsEventDepartmentLimitService, SportsEventDepartmentLimitService>();
builder.Services.AddScoped<ISportsRegistrationService, SportsRegistrationService>();

// =========================================================
// Students
// =========================================================

builder.Services.AddScoped<IStudentRepository, StudentRepository>();
builder.Services.AddScoped<IStudentMasterListRepository, StudentMasterListRepository>();

builder.Services.AddScoped<IStudentService, StudentService>();
builder.Services.AddScoped<IStudentMasterListService, StudentMasterListService>();

builder.Services.AddScoped<
    IStudentRegistrationService,
    StudentRegistrationService>();

// =========================================================
// OTP VERIFICATION
// =========================================================
builder.Services.AddScoped<IOtpRepository, OtpRepository>();
builder.Services.AddScoped<IOtpService, OtpService>();

// =========================================================
// System Settings
// =========================================================

builder.Services.AddScoped<ISystemSettingRepository, SystemSettingRepository>();
builder.Services.AddScoped<ISystemSettingService, SystemSettingService>();

// =========================================================
// Swagger / OpenAPI
// =========================================================

builder.Services.AddEndpointsApiExplorer();


builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "Enter your JWT token."
    });

    options.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

// =========================================================
// Build Application
// =========================================================

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider
        .GetRequiredService<CampusDbContext>();

    await DatabaseSeeder.SeedAsync(context);
}

// =========================================================
// HTTP Request Pipeline
// =========================================================


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();