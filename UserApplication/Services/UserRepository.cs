using Microsoft.EntityFrameworkCore;
using UserApplication.DbContexts;
using UserApplication.Entities;
using UserApplication.Models;

namespace UserApplication.Services
{
    public class UserRepository : IUserRepository
    {
        private readonly UserInfoContext _userInfoContext;
        public UserRepository(UserInfoContext context) {
            _userInfoContext = context ?? throw new ArgumentNullException(nameof(context));
        }
        public async Task CreateUserAsync(User userData)
        {
             await _userInfoContext.Users.AddAsync(userData);
        }

        public void DeleteUser(User user)
        {
            _userInfoContext.Users.Remove(user);
        }

        public async Task<User?> GetUserByIdAsync(int userId)
        {
            return await _userInfoContext.Users.Where(user => user.Id == userId).FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<User>> GetUsersAsync()
        {
            return await _userInfoContext.Users.OrderBy(user => user.Name).ToListAsync();
        }

        public async Task<(IEnumerable<User>, PaginationMetadata)> GetUsersAsync(string? name, int pageNumber, int pageSize)
        {
            //if (string.IsNullOrEmpty(name))
            //{
            //    return await GetUsersAsync();
            //}

            var userCollection = _userInfoContext.Users as IQueryable<User>;

            if (!string.IsNullOrEmpty(name))
            {
                name = name.Trim();
                userCollection = userCollection.Where(user => user.Name.Contains(name));
            }

            var totalItemCount = await userCollection.CountAsync();

            var paginationMetadata = new PaginationMetadata(totalItemCount, pageSize, pageNumber);


            var userData =  await userCollection
                .OrderBy(user => user.Name)
                .Skip(pageSize * (pageNumber -1))
                .Take(pageSize)
                .ToListAsync();

            return (userData, paginationMetadata);
        }

        public async Task<bool> SaveChangesAsync()
        {
            return (await _userInfoContext.SaveChangesAsync() >= 0);
        }

        public async Task<bool> UserExistAsync(int userId)
        {
            return await _userInfoContext.Users.AnyAsync(user =>  user.Id == userId);
        }

        public async Task<User?> ValidateUserAsync(string userName, string password)
        {
             return await _userInfoContext.Users
                .Where(user => user.Name == userName && user.Password == password).FirstOrDefaultAsync();
        }
    }
}
