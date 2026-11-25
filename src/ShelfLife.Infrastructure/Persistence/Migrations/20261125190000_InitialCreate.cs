namespace ShelfLife.Infrastructure.Persistence.Migrations;

using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using ShelfLife.Infrastructure.Persistence;

[DbContext(typeof(ShelfLifeDbContext))]
[Migration("20261125190000_InitialCreate")]
public sealed class InitialCreate : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "inventory_items",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                Name = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                Quantity = table.Column<decimal>(type: "numeric(12,3)", precision: 12, scale: 3, nullable: false),
                Unit = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                ExpiryDate = table.Column<DateOnly>(type: "date", nullable: false),
                Category = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
            },
            constraints: table => table.PrimaryKey("PK_inventory_items", x => x.Id));

        migrationBuilder.CreateIndex(
            name: "IX_inventory_items_Category",
            table: "inventory_items",
            column: "Category");

        migrationBuilder.CreateIndex(
            name: "IX_inventory_items_ExpiryDate",
            table: "inventory_items",
            column: "ExpiryDate");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(name: "inventory_items");
    }
}
