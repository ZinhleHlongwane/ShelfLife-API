namespace ShelfLife.API.Contracts.Inventory;

using System.ComponentModel.DataAnnotations;
using ShelfLife.Domain.Enums;

public sealed record InventoryItemRequest(
    [property: Required, StringLength(120, MinimumLength = 1)] string Name,
    [property: Range(typeof(decimal), "0.001", "999999999")] decimal Quantity,
    [property: EnumDataType(typeof(MeasurementUnit))] MeasurementUnit Unit,
    DateOnly ExpiryDate,
    [property: EnumDataType(typeof(StorageCategory))] StorageCategory Category);
