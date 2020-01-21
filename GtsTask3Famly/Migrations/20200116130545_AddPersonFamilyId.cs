using Microsoft.EntityFrameworkCore.Migrations;

namespace GtsTask3Famly.Migrations
{
    public partial class AddPersonFamilyId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "FamilyId",
                table: "People",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_People_FamilyId",
                table: "People",
                column: "FamilyId");

            migrationBuilder.AddForeignKey(
                name: "FK_People_Families_FamilyId",
                table: "People",
                column: "FamilyId",
                principalTable: "Families",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_People_Families_FamilyId",
                table: "People");

            migrationBuilder.DropIndex(
                name: "IX_People_FamilyId",
                table: "People");

            migrationBuilder.DropColumn(
                name: "FamilyId",
                table: "People");
        }
    }
}
