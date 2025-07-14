using Dev.Core.Domain;
using Dev.Core.Interfaces;

using Microsoft.EntityFrameworkCore;

using System.Reflection;

namespace Dev.Infrastructure;

public class CoreDbContext: DbContext, ICoreDbContext
{
    public DbSet<Setting> Settings => Set<Setting>();

    public CoreDbContext(DbContextOptions<CoreDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("core");
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
    public string GenerateCreateScript()
    {
        return base.Database.GenerateCreateScript();
    }

    public async Task SaveChangesAsync()
    {
        await base.SaveChangesAsync();
    }
}
