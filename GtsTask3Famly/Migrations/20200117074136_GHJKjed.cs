using Microsoft.EntityFrameworkCore.Migrations;

namespace GtsTask3Famly.Migrations
{
    public partial class GHJKjed : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "RelRoles",
                columns: new[] { "Id", "GenderId", "Name" },
                values: new object[] { 15, null, "norole" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "RelRoles",
                keyColumn: "Id",
                keyValue: 15);
        }
    }
}
