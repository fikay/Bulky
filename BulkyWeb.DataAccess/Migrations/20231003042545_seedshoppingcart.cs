using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BulkyWeb.Migrations
{
    /// <inheritdoc />
    public partial class seedshoppingcart : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "ShoppingCarts",
                columns: new[] { "Id", "ApplicationUserId", "Count", "ProductId" },
                values: new object[] { 3, "32628229", 5, 4444 });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "ShoppingCarts",
                keyColumn: "Id",
                keyValue: 3);
        }
    }
}
