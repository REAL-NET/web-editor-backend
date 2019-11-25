using Auth.Models;
using Microsoft.EntityFrameworkCore;

namespace Auth.Data
{
    public class UsersDbContext : DbContext
    {
        public UsersDbContext(DbContextOptions<UsersDbContext> options)
            : base(options)
        {
        }
        
        public DbSet<UserInfo> Users { get; set; }
    }
}