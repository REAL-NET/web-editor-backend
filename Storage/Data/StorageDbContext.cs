using Microsoft.EntityFrameworkCore;

namespace Storage.Data
{
    public class StorageDbContext : DbContext
    {
        public StorageDbContext(DbContextOptions<StorageDbContext> options)
            : base(options)
        {
        }
        
        public DbSet<Models.Database.DbRecord> StorageDb { get; set; }
    }
}