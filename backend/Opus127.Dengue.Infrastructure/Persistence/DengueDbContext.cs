using Microsoft.EntityFrameworkCore;
using Opus127.Dengue.Domain.Entities;

namespace Opus127.Dengue.Infrastructure.Persistence;

public sealed class DengueDbContext(DbContextOptions<DengueDbContext> options) : DbContext(options)
{
    public DbSet<DengueRecord> DengueRecords => Set<DengueRecord>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(DengueDbContext).Assembly);
    }
}
