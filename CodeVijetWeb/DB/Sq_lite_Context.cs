using Microsoft.EntityFrameworkCore;

namespace CodeVijetWeb.DB
{
    public class Sq_lite_Context : DbContext
    {

        public Sq_lite_Context()
            => Database.MigrateAsync();
        
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Filename=historyApp.db");
        }

        public DbSet<Log> Logs { get; set; }   
    }
}
