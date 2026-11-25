using Microsoft.EntityFrameworkCore;
using ShelfLife.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("ShelfLife")
    ?? throw new InvalidOperationException("Connection string 'ShelfLife' is not configured.");

builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddDbContext<ShelfLifeDbContext>(options =>
    options.UseNpgsql(connectionString));

builder.Services.AddCors(options =>
{
    options.AddPolicy("ShelfLifeClients", policy =>
        policy.WithOrigins(
                "http://localhost:5173",
                "http://127.0.0.1:5173")
            .AllowAnyHeader()
            .AllowAnyMethod());
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseCors("ShelfLifeClients");
app.MapControllers();
app.MapGet("/health", () => Results.Ok(new { status = "healthy" }));

app.Run();

public partial class Program;
