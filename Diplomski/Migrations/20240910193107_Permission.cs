using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Diplomski.Migrations
{
    /// <inheritdoc />
    public partial class Permission : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PermissionPermissionSet_Permission_Permissionsid",
                table: "PermissionPermissionSet");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Permission",
                table: "Permission");

            migrationBuilder.RenameTable(
                name: "Permission",
                newName: "Permissions");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Permissions",
                table: "Permissions",
                column: "id");

            migrationBuilder.AddForeignKey(
                name: "FK_PermissionPermissionSet_Permissions_Permissionsid",
                table: "PermissionPermissionSet",
                column: "Permissionsid",
                principalTable: "Permissions",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PermissionPermissionSet_Permissions_Permissionsid",
                table: "PermissionPermissionSet");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Permissions",
                table: "Permissions");

            migrationBuilder.RenameTable(
                name: "Permissions",
                newName: "Permission");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Permission",
                table: "Permission",
                column: "id");

            migrationBuilder.AddForeignKey(
                name: "FK_PermissionPermissionSet_Permission_Permissionsid",
                table: "PermissionPermissionSet",
                column: "Permissionsid",
                principalTable: "Permission",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
