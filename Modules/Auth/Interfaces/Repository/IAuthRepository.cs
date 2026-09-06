using CampusServicePortal.Modules.Identity.Entities;


namespace CampusServicePortal_TicUnicorns.Modules.Auth.Interfaces.Repository;

public interface IAuthRepository
{
    Task<User?> GetUserByUsernameAsync(string username);

    Task<User?> GetUserByIdAsync(int userId);

    Task UpdateUserAsync(User user);

    Task<bool> UserExistsAsync(int userId);

    Task<bool> UsernameExistsAsync(string username);
}