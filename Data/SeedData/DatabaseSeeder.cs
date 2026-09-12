using CampusServicePortal.Modules.Identity.Entities;
using CampusServicePortal_TicUnicorns.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CampusServicePortal_TicUnicorns.Data.SeedData;

public static class DatabaseSeeder
{
    public static async Task SeedAsync(CampusDbContext context)
    {
        // ==============================
        // 1. Seed Admin Role
        // ==============================

        var adminRole = await context.Roles
            .FirstOrDefaultAsync(r => r.RoleName == "Admin");

        if (adminRole is null)
        {
            adminRole = new Role
            {
                RoleName = "Admin",
                Description = "System administrator",
                IsSystemRole = true,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            await context.Roles.AddAsync(adminRole);
            await context.SaveChangesAsync();
        }

        // ==============================
        // 2. Seed Student Role
        // ==============================

        var studentRole = await context.Roles
            .FirstOrDefaultAsync(r => r.RoleName == "Student");

        if (studentRole is null)
        {
            studentRole = new Role
            {
                RoleName = "Student",
                Description = "Student user",
                IsSystemRole = true,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            await context.Roles.AddAsync(studentRole);
            await context.SaveChangesAsync();
        }

        // ==============================
        // 3. Seed Admin User
        // ==============================

        const string username = "admin";
        const string password = "Admin@123";

        var adminUser = await context.Users
            .FirstOrDefaultAsync(u => u.Username == username);

        if (adminUser is null)
        {
            adminUser = new User
            {
                Username = username,
                Email = "admin@campus.local",
                PhoneNumber = null,
                IsPhoneVerified = true,
                MustChangePassword = false,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            var passwordHasher = new PasswordHasher<User>();

            adminUser.PasswordHash = passwordHasher.HashPassword(
                adminUser,
                password);

            await context.Users.AddAsync(adminUser);
            await context.SaveChangesAsync();
        }

        // ==============================
        // 4. Assign Admin Role
        // ==============================

        var userRoleExists = await context.UserRoles
            .AnyAsync(ur =>
                ur.UserId == adminUser.UserId &&
                ur.RoleId == adminRole.RoleId);

        if (!userRoleExists)
        {
            var userRole = new UserRole
            {
                UserId = adminUser.UserId,
                RoleId = adminRole.RoleId,
                AssignedAt = DateTime.UtcNow
            };

            await context.UserRoles.AddAsync(userRole);
            await context.SaveChangesAsync();
        }
    }
}