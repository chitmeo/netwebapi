using Dev.Core.Domain;

using Microsoft.EntityFrameworkCore;

namespace Dev.Core.Interfaces;

public interface ICoreDbContext
{
    DbSet<Setting> Settings { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    Task SaveChangesAsync();
    string GenerateCreateScript();
}
