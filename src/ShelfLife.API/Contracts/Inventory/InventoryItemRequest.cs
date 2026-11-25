namespace ShelfLife.API.Contracts.Inventory;

using System.ComponentModel.DataAnnotations;
using ShelfLife.Domain.Enums;

public sealed record InventoryItemRequest
{
    [Required]
    [StringLength(120, MinimumLength = 1)]
    public string Name { get; init; } = string.Empty;

    [Range(typeof(decimal), "0.001", "999999999")]
    public decimal Quantity { get; init; }

    [EnumDataType(typeof(MeasurementUnit))]
    public MeasurementUnit Unit { get; init; }

    public DateOnly ExpiryDate { get; init; }

    [EnumDataType(typeof(StorageCategory))]
    public StorageCategory Category { get; init; }
}
