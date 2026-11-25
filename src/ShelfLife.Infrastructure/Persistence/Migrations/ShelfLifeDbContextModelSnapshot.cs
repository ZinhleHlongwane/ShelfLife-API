namespace ShelfLife.Infrastructure.Persistence.Migrations;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using ShelfLife.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore.Metadata;
using ShelfLife.Domain.Entities;
using ShelfLife.Domain.Enums;

[DbContext(typeof(ShelfLifeDbContext))]
public sealed class ShelfLifeDbContextModelSnapshot : ModelSnapshot
{
    protected override void BuildModel(ModelBuilder modelBuilder)
    {
        modelBuilder
            .HasAnnotation("ProductVersion", "10.0.15")
            .HasAnnotation("Relational:MaxIdentifierLength", 63);

        modelBuilder.Entity<InventoryItem>(entity =>
        {
            entity.HasKey(item => item.Id).HasName("PK_inventory_items");

            entity.ToTable("inventory_items");

            entity.HasIndex(item => item.Category)
                .HasDatabaseName("IX_inventory_items_Category");

            entity.HasIndex(item => item.ExpiryDate)
                .HasDatabaseName("IX_inventory_items_ExpiryDate");

            entity.Property(item => item.Id)
                .HasColumnType("uuid");

            entity.Property(item => item.Name)
                .IsRequired()
                .HasMaxLength(120)
                .HasColumnType("character varying(120)");

            entity.Property(item => item.Quantity)
                .HasPrecision(12, 3)
                .HasColumnType("numeric(12,3)");

            entity.Property(item => item.Unit)
                .IsRequired()
                .HasMaxLength(20)
                .HasColumnType("character varying(20)")
                .HasConversion<string>();

            entity.Property(item => item.ExpiryDate)
                .IsRequired()
                .HasColumnType("date");

            entity.Property(item => item.Category)
                .IsRequired()
                .HasMaxLength(20)
                .HasColumnType("character varying(20)")
                .HasConversion<string>();

            entity.Property(item => item.CreatedAt)
                .IsRequired()
                .HasColumnType("timestamp with time zone");

            entity.Property(item => item.UpdatedAt)
                .IsRequired()
                .HasColumnType("timestamp with time zone");
        });
    }
}
