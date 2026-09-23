using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TravoRides.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class PaymentMigration_3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Rate",
                table: "Bookings",
                newName: "TotalAmount");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "TotalAmount",
                table: "Bookings",
                newName: "Rate");
        }
    }
}
