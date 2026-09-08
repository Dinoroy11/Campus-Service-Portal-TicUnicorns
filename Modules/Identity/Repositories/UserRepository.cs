using CampusServicePortal.Modules.Identity.Entities;
using CampusServicePortal.Modules.Identity.Interfaces.Repository;


namespace CampusServicePortal_TicUnicorns.Modules.Identity.Repositories;

public class UserRepository : IUserRepository
{
    public Task<User?> GetByIdAsync(int userId)
    {
        throw new NotImplementedException();
    }

    public Task<User?> GetByUsernameAsync(string username)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<User>> GetAllAsync()
    {
        throw new NotImplementedException();
    }

    public Task AddAsync(User user)
    {
        throw new NotImplementedException();
    }

    public Task UpdateAsync(User user)
    {
        throw new NotImplementedException();
    }

    public Task<bool> ExistsAsync(int userId)
    {
        throw new NotImplementedException();
    }
}