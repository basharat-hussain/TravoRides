using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TravoRides.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class DiscountPrice : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "Discount",
                table: "Transit",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "Price",
                table: "Transit",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Discount",
                table: "Transit");

            migrationBuilder.DropColumn(
                name: "Price",
                table: "Transit");
        }
    }
}
