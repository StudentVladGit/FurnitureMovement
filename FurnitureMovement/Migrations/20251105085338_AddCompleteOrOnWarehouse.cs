using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FurnitureMovement.Migrations
{
    /// <inheritdoc />
    public partial class AddCompleteOrOnWarehouse : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CompleteOrOnWarehouseIndicator",
                table: "Orders",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CompleteOrOnWarehouseIndicator",
                table: "Orders");
        }
    }
}
