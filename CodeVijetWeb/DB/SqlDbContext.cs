using Microsoft.EntityFrameworkCore;

namespace CodeVijetWeb.DB
{
    public class SqlDbContext : DbContext
    {

        public SqlDbContext()
            => Database.MigrateAsync();
        
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Filename=historyApp.db");
        }

        public DbSet<Log> Logs { get; set; }   
    }
}
