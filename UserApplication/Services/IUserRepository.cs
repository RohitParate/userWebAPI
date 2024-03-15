using UserApplication.Entities;
using UserApplication.Models;

namespace UserApplication.Services
{
    public interface IUserRepository
    {
        Task<IEnumerable<User>> GetUsersAsync();
        Task<(IEnumerable<User>, PaginationMetadata)> GetUsersAsync(string? name, int pageNumber, int pageSize);

        Task<bool> UserExistAsync(int userId);

        Task<User?> GetUserByIdAsync(int userId);

        Task CreateUserAsync(User userData);

        Task<bool> SaveChangesAsync();

        void DeleteUser(User user);

        Task<User?> ValidateUserAsync(string userName, string password);

    }
}
