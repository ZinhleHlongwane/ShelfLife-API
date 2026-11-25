namespace ShelfLife.Infrastructure.Persistence.Configurations;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ShelfLife.Domain.Entities;
using ShelfLife.Domain.Enums;

public sealed class InventoryItemConfiguration : IEntityTypeConfiguration<InventoryItem>
{
    public void Configure(EntityTypeBuilder<InventoryItem> builder)
    {
        builder.ToTable("inventory_items");
        builder.HasKey(item => item.Id);

        builder.Property(item => item.Name)
            .HasMaxLength(120)
            .IsRequired();

        builder.Property(item => item.Quantity)
            .HasPrecision(12, 3)
            .IsRequired();

        builder.Property(item => item.Unit)
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(item => item.ExpiryDate)
            .HasColumnType("date")
            .IsRequired();

        builder.Property(item => item.Category)
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(item => item.CreatedAt).IsRequired();
        builder.Property(item => item.UpdatedAt).IsRequired();

        builder.HasIndex(item => item.ExpiryDate);
        builder.HasIndex(item => item.Category);
    }
}
