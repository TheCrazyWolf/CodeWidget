using Microsoft.EntityFrameworkCore;

namespace CodeVijetWeb.DB
{
    public class Sq_lite_Context : DbContext 
    {
      
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Filename=historyApp.db");
        }

        public DbSet<Logs> Logs { get; set; }   
    }
}
