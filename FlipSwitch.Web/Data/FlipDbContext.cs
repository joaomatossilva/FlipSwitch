namespace FlipSwitch.Web.Data;

using Microsoft.EntityFrameworkCore;

public class FlipDbContext : DbContext
{
    public FlipDbContext(DbContextOptions<FlipDbContext> options)
        : base(options)
    {
    }

    public DbSet<Config> Configs { get; set; }
}