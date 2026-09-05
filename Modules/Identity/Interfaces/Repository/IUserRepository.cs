using CampusServicePortal.Modules.Identity.Entities;

namespace CampusServicePortal.Modules.Identity.Interfaces.Repository;

public interface IUserRepository
{
    Task<User?> GetByIdAsync(int userId);

    Task<User?> GetByUsernameAsync(string username);

    Task<IEnumerable<User>> GetAllAsync();

    Task AddAsync(User user);

    Task UpdateAsync(User user);

    Task<bool> ExistsAsync(int userId);
}