using Microsoft.EntityFrameworkCore.Migrations;

namespace GbayWebApp.Migrations
{
    public partial class AddSecurityQuestions : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SecurityQuestion1",
                table: "AspNetUsers",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SecurityQuestion2",
                table: "AspNetUsers",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            
        }
    }
}
