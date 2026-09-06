using CampusServicePortal.Modules.Identity.Entities;
using CampusServicePortal_TicUnicorns.Modules.Auth.Interfaces.Repository;

namespace CampusServicePortal_TicUnicorns.Modules.Auth.Repositories;

public class AuthRepository : IAuthRepository
{
    public Task<User?> GetUserByUsernameAsync(string username)
    {
        throw new NotImplementedException();
    }

    public Task<User?> GetUserByIdAsync(int userId)
    {
        throw new NotImplementedException();
    }

    public Task UpdateUserAsync(User user)
    {
        throw new NotImplementedException();
    }

    public Task<bool> UserExistsAsync(int userId)
    {
        throw new NotImplementedException();
    }

    public Task<bool> UsernameExistsAsync(string username)
    {
        throw new NotImplementedException();
    }
}
