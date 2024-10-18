using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FurnitureMovement.Migrations
{
    /// <inheritdoc />
    public partial class init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Orders",
                columns: table => new
                {
                    Number = table.Column<string>(type: "text", nullable: false),
                    Drawning = table.Column<string>(type: "text", nullable: false),
                    Furniture = table.Column<string>(type: "text", nullable: false),
                    AdmissionDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    Quantity = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Orders", x => x.Number);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Orders");
        }
    }
}
