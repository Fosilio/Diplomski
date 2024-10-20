using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Diplomski.Migrations
{
    /// <inheritdoc />
    public partial class Relation2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            /*migrationBuilder.DropForeignKey(
                name: "FK_Users_PermissionSets_PermissionSetId",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Users_PermissionSetId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "PermissionSetId",
                table: "Users");*/
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            /*migrationBuilder.AddColumn<int>(
                name: "PermissionSetId",
                table: "Users",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Users_PermissionSetId",
                table: "Users",
                column: "PermissionSetId");

            migrationBuilder.AddForeignKey(
                name: "FK_Users_PermissionSets_PermissionSetId",
                table: "Users",
                column: "PermissionSetId",
                principalTable: "PermissionSets",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);*/
        }
    }
}
