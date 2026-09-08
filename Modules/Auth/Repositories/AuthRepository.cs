using CampusServicePortal.Modules.Identity.Entities;
using CampusServicePortal_TicUnicorns.Data;
using CampusServicePortal_TicUnicorns.Modules.Auth.Interfaces.Repository;
using Microsoft.EntityFrameworkCore;

namespace CampusServicePortal_TicUnicorns.Modules.Auth.Repositories;

public class AuthRepository : IAuthRepository
{
    private readonly CampusDbContext _context;

    public AuthRepository(CampusDbContext context)
    {
        _context = context;
    }

    public async Task<User?> GetUserByUsernameAsync(string username)
    {
        return await _context.Users
            .FirstOrDefaultAsync(u => u.Username == username);
    }

    public async Task<User?> GetUserByIdAsync(int userId)
    {
        return await _context.Users
            .FirstOrDefaultAsync(u => u.UserId == userId);
    }

    public async Task UpdateUserAsync(User user)
    {
        _context.Users.Update(user);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> UserExistsAsync(int userId)
    {
        return await _context.Users
            .AnyAsync(u => u.UserId == userId);
    }

    public async Task<bool> UsernameExistsAsync(string username)
    {
        return await _context.Users
            .AnyAsync(u => u.Username == username);
    }
}