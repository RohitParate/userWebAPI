using Microsoft.EntityFrameworkCore;
using UserApplication.Entities;

namespace UserApplication.DbContexts
{
    public class UserInfoContext : DbContext // derived from DbContext that represent database context , we can also use multiple contexts
    {
        public DbSet<User> Users { get; set; }

        public UserInfoContext(DbContextOptions<UserInfoContext> options) : base(options) { }
    }
}
