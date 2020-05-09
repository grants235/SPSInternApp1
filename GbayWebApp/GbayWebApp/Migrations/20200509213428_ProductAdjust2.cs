using Microsoft.EntityFrameworkCore.Migrations;

namespace GbayWebApp.Migrations
{
    public partial class ProductAdjust2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "Seller",
                table: "Products",
                nullable: false,
                oldClrType: typeof(float),
                oldType: "real");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<float>(
                name: "Seller",
                table: "Products",
                type: "real",
                nullable: false,
                oldClrType: typeof(decimal));
        }
    }
}
