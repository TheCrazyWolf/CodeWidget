using Microsoft.EntityFrameworkCore;

namespace CodeShare.DB;

public sealed class SqlDbContext : DbContext
{
    public DbSet<Log> Logs { get; set; }

    public SqlDbContext()
        => Database.MigrateAsync();

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite("Data Source = app.db");
    }
}