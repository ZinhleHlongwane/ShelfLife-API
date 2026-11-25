namespace ShelfLife.Domain.Entities;

using ShelfLife.Domain.Enums;

public sealed class InventoryItem
{
    private InventoryItem() { }

    public Guid Id { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public decimal Quantity { get; private set; }
    public MeasurementUnit Unit { get; private set; }
    public DateOnly ExpiryDate { get; private set; }
    public StorageCategory Category { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset UpdatedAt { get; private set; }

    public static InventoryItem Create(
        string name,
        decimal quantity,
        MeasurementUnit unit,
        DateOnly expiryDate,
        StorageCategory category,
        DateTimeOffset? now = null)
    {
        Validate(name, quantity);

        var timestamp = now ?? DateTimeOffset.UtcNow;
        return new InventoryItem
        {
            Id = Guid.NewGuid(),
            Name = name.Trim(),
            Quantity = quantity,
            Unit = unit,
            ExpiryDate = expiryDate,
            Category = category,
            CreatedAt = timestamp,
            UpdatedAt = timestamp
        };
    }

    public void Update(
        string name,
        decimal quantity,
        MeasurementUnit unit,
        DateOnly expiryDate,
        StorageCategory category,
        DateTimeOffset? now = null)
    {
        Validate(name, quantity);

        Name = name.Trim();
        Quantity = quantity;
        Unit = unit;
        ExpiryDate = expiryDate;
        Category = category;
        UpdatedAt = now ?? DateTimeOffset.UtcNow;
    }

    private static void Validate(string name, decimal quantity)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("A food name is required.", nameof(name));

        if (name.Trim().Length > 120)
            throw new ArgumentException("Food name cannot exceed 120 characters.", nameof(name));

        if (quantity <= 0)
            throw new ArgumentOutOfRangeException(nameof(quantity), "Quantity must be greater than zero.");
    }
}
