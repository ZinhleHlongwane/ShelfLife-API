namespace ShelfLife.API.Contracts.Inventory;

using ShelfLife.Domain.Entities;
using ShelfLife.Domain.Enums;

public sealed record InventoryItemResponse(
    Guid Id,
    string Name,
    decimal Quantity,
    MeasurementUnit Unit,
    DateOnly ExpiryDate,
    StorageCategory Category,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt)
{
    public static InventoryItemResponse FromEntity(InventoryItem item) =>
        new(item.Id, item.Name, item.Quantity, item.Unit, item.ExpiryDate,
            item.Category, item.CreatedAt, item.UpdatedAt);
}
