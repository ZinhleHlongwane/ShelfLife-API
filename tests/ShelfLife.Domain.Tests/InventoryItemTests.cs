namespace ShelfLife.Domain.Tests;

using ShelfLife.Domain.Entities;
using ShelfLife.Domain.Enums;
using Xunit;

public sealed class InventoryItemTests
{
    [Fact]
    public void Create_TrimsName_AndStoresInventoryValues()
    {
        var now = new DateTimeOffset(2026, 11, 25, 10, 0, 0, TimeSpan.Zero);
        var expiryDate = new DateOnly(2026, 12, 1);

        var item = InventoryItem.Create(
            "  Milk  ", 2m, MeasurementUnit.Liter, expiryDate,
            StorageCategory.Fridge, now);

        Assert.NotEqual(Guid.Empty, item.Id);
        Assert.Equal("Milk", item.Name);
        Assert.Equal(2m, item.Quantity);
        Assert.Equal(MeasurementUnit.Liter, item.Unit);
        Assert.Equal(expiryDate, item.ExpiryDate);
        Assert.Equal(StorageCategory.Fridge, item.Category);
        Assert.Equal(now, item.CreatedAt);
        Assert.Equal(now, item.UpdatedAt);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_RejectsBlankName(string name)
    {
        Assert.Throws<ArgumentException>(() =>
            InventoryItem.Create(name, 1m, MeasurementUnit.Count,
                new DateOnly(2026, 12, 1), StorageCategory.Pantry));
    }

    [Fact]
    public void Create_RejectsNonPositiveQuantity()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            InventoryItem.Create("Milk", 0m, MeasurementUnit.Liter,
                new DateOnly(2026, 12, 1), StorageCategory.Fridge));
    }

    [Fact]
    public void Update_ChangesValues_AndPreservesCreatedAt()
    {
        var created = new DateTimeOffset(2026, 11, 20, 10, 0, 0, TimeSpan.Zero);
        var updated = created.AddDays(1);
        var item = InventoryItem.Create("Milk", 1m, MeasurementUnit.Liter,
            new DateOnly(2026, 12, 1), StorageCategory.Fridge, created);

        item.Update("Oat milk", 2m, MeasurementUnit.Count,
            new DateOnly(2026, 12, 5), StorageCategory.Pantry, updated);

        Assert.Equal("Oat milk", item.Name);
        Assert.Equal(2m, item.Quantity);
        Assert.Equal(MeasurementUnit.Count, item.Unit);
        Assert.Equal(new DateOnly(2026, 12, 5), item.ExpiryDate);
        Assert.Equal(StorageCategory.Pantry, item.Category);
        Assert.Equal(created, item.CreatedAt);
        Assert.Equal(updated, item.UpdatedAt);
    }
}
