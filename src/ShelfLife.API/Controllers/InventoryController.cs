namespace ShelfLife.API.Controllers;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShelfLife.API.Contracts.Inventory;
using ShelfLife.Domain.Entities;
using ShelfLife.Infrastructure.Persistence;

[ApiController]
[Route("api/[controller]")]
public sealed class InventoryController(ShelfLifeDbContext dbContext) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType<IReadOnlyList<InventoryItemResponse>>(StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<InventoryItemResponse>>> GetAll(
        [FromQuery] string? category,
        CancellationToken cancellationToken)
    {
        var query = dbContext.InventoryItems.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(category) &&
            Enum.TryParse<Domain.Enums.StorageCategory>(category, true, out var parsedCategory))
        {
            query = query.Where(item => item.Category == parsedCategory);
        }

        var items = await query
            .OrderBy(item => item.ExpiryDate)
            .ThenBy(item => item.Name)
            .Select(item => InventoryItemResponse.FromEntity(item))
            .ToListAsync(cancellationToken);

        return Ok(items);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType<InventoryItemResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<InventoryItemResponse>> GetById(
        Guid id,
        CancellationToken cancellationToken)
    {
        var item = await dbContext.InventoryItems.AsNoTracking()
            .FirstOrDefaultAsync(candidate => candidate.Id == id, cancellationToken);

        return item is null ? NotFound() : Ok(InventoryItemResponse.FromEntity(item));
    }

    [HttpPost]
    [ProducesResponseType<InventoryItemResponse>(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<InventoryItemResponse>> Create(
        InventoryItemRequest request,
        CancellationToken cancellationToken)
    {
        var item = InventoryItem.Create(request.Name, request.Quantity, request.Unit,
            request.ExpiryDate, request.Category);

        dbContext.InventoryItems.Add(item);
        await dbContext.SaveChangesAsync(cancellationToken);

        var response = InventoryItemResponse.FromEntity(item);
        return CreatedAtAction(nameof(GetById), new { id = item.Id }, response);
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType<InventoryItemResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<InventoryItemResponse>> Update(
        Guid id,
        InventoryItemRequest request,
        CancellationToken cancellationToken)
    {
        var item = await dbContext.InventoryItems
            .FirstOrDefaultAsync(candidate => candidate.Id == id, cancellationToken);

        if (item is null)
            return NotFound();

        item.Update(request.Name, request.Quantity, request.Unit,
            request.ExpiryDate, request.Category);

        await dbContext.SaveChangesAsync(cancellationToken);
        return Ok(InventoryItemResponse.FromEntity(item));
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var item = await dbContext.InventoryItems
            .FirstOrDefaultAsync(candidate => candidate.Id == id, cancellationToken);

        if (item is null)
            return NotFound();

        dbContext.InventoryItems.Remove(item);
        await dbContext.SaveChangesAsync(cancellationToken);
        return NoContent();
    }

    [HttpGet("expiring")]
    [ProducesResponseType<IReadOnlyList<InventoryItemResponse>>(StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<InventoryItemResponse>>> GetExpiring(
        [FromQuery] int withinDays = 3,
        CancellationToken cancellationToken = default)
    {
        if (withinDays is < 0 or > 365)
            return BadRequest("withinDays must be between 0 and 365.");

        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var cutoff = today.AddDays(withinDays);

        var items = await dbContext.InventoryItems.AsNoTracking()
            .Where(item => item.ExpiryDate >= today && item.ExpiryDate <= cutoff)
            .OrderBy(item => item.ExpiryDate)
            .Select(item => InventoryItemResponse.FromEntity(item))
            .ToListAsync(cancellationToken);

        return Ok(items);
    }
}
