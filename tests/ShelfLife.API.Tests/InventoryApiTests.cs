namespace ShelfLife.API.Tests;

using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using ShelfLife.API.Contracts.Inventory;
using ShelfLife.Domain.Enums;
using ShelfLife.Infrastructure.Persistence;
using Xunit;

public sealed class InventoryApiTests
{
    [Fact]
    public async Task InventoryEndpoints_CreateReadFilterUpdateAndDeleteItem()
    {
        var connectionString = Environment.GetEnvironmentVariable("SHELFLIFE_TEST_CONNECTION");
        Assert.False(string.IsNullOrWhiteSpace(connectionString),
            "SHELFLIFE_TEST_CONNECTION must point to the PostgreSQL integration-test database.");

        using var factory = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder =>
            {
                builder.UseEnvironment("Testing");
                builder.ConfigureServices(services =>
                {
                    services.RemoveAll<DbContextOptions<ShelfLifeDbContext>>();
                    services.RemoveAll<ShelfLifeDbContext>();
                    services.AddDbContext<ShelfLifeDbContext>(options =>
                        options.UseNpgsql(connectionString));
                });
            });

        using var client = factory.CreateClient();
        using var scope = factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ShelfLifeDbContext>();

        await db.Database.MigrateAsync();
        await db.InventoryItems.ExecuteDeleteAsync();

        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var request = new InventoryItemRequest
        {
            Name = "Milk",
            Quantity = 2m,
            Unit = MeasurementUnit.Liter,
            ExpiryDate = today.AddDays(2),
            Category = StorageCategory.Fridge
        };

        var createResponse = await client.PostAsJsonAsync("/api/inventory", request, JsonOptions);
        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);

        var created = await createResponse.Content.ReadFromJsonAsync<InventoryItemResponse>(JsonOptions);
        Assert.NotNull(created);
        Assert.Equal("Milk", created.Name);
        Assert.Equal(2m, created.Quantity);
        Assert.Equal(MeasurementUnit.Liter, created.Unit);
        Assert.Equal(today.AddDays(2), created.ExpiryDate);
        Assert.Equal(StorageCategory.Fridge, created.Category);

        var getResponse = await client.GetAsync($"/api/inventory/{created.Id}");
        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);

        var list = await client.GetFromJsonAsync<List<InventoryItemResponse>>("/api/inventory", JsonOptions);
        Assert.NotNull(list);
        Assert.Single(list);

        var filtered = await client.GetFromJsonAsync<List<InventoryItemResponse>>(
            "/api/inventory?category=Fridge", JsonOptions);
        Assert.NotNull(filtered);
        Assert.Single(filtered);

        var invalidFilter = await client.GetAsync("/api/inventory?category=Garage");
        Assert.Equal(HttpStatusCode.BadRequest, invalidFilter.StatusCode);

        var expiring = await client.GetFromJsonAsync<List<InventoryItemResponse>>(
            "/api/inventory/expiring?withinDays=3", JsonOptions);
        Assert.NotNull(expiring);
        Assert.Single(expiring);

        var updateRequest = request with
        {
            Name = "Oat milk",
            Quantity = 1m,
            Unit = MeasurementUnit.Count,
            Category = StorageCategory.Pantry
        };
        var updateResponse = await client.PutAsJsonAsync(
            $"/api/inventory/{created.Id}", updateRequest, JsonOptions);
        Assert.Equal(HttpStatusCode.OK, updateResponse.StatusCode);

        var updated = await updateResponse.Content.ReadFromJsonAsync<InventoryItemResponse>(JsonOptions);
        Assert.NotNull(updated);
        Assert.Equal("Oat milk", updated.Name);
        Assert.Equal(StorageCategory.Pantry, updated.Category);

        var invalidRequest = request with { Quantity = 0m };
        var invalidResponse = await client.PostAsJsonAsync("/api/inventory", invalidRequest, JsonOptions);
        Assert.Equal(HttpStatusCode.BadRequest, invalidResponse.StatusCode);

        var deleteResponse = await client.DeleteAsync($"/api/inventory/{created.Id}");
        Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);

        var missingResponse = await client.GetAsync($"/api/inventory/{created.Id}");
        Assert.Equal(HttpStatusCode.NotFound, missingResponse.StatusCode);
    }

    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web)
    {
        Converters = { new JsonStringEnumConverter() }
    };
}
