namespace ShelfLife.Infrastructure.Persistence;

using Microsoft.EntityFrameworkCore;
using ShelfLife.Domain.Entities;

public sealed class ShelfLifeDbContext(DbContextOptions<ShelfLifeDbContext> options)
    : DbContext(options)
{
    public DbSet<InventoryItem> InventoryItems => Set<InventoryItem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ShelfLifeDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}
