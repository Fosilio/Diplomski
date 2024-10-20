using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Diplomski.Migrations
{
    /// <inheritdoc />
    public partial class PermissionPermissionSetRelation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Permission",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    description = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Permission", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "PermissionPermissionSet",
                columns: table => new
                {
                    PermissionSetsId = table.Column<int>(type: "int", nullable: false),
                    Permissionsid = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PermissionPermissionSet", x => new { x.PermissionSetsId, x.Permissionsid });
                    table.ForeignKey(
                        name: "FK_PermissionPermissionSet_PermissionSets_PermissionSetsId",
                        column: x => x.PermissionSetsId,
                        principalTable: "PermissionSets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PermissionPermissionSet_Permission_Permissionsid",
                        column: x => x.Permissionsid,
                        principalTable: "Permission",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PermissionPermissionSet_Permissionsid",
                table: "PermissionPermissionSet",
                column: "Permissionsid");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PermissionPermissionSet");

            migrationBuilder.DropTable(
                name: "Permission");
        }
    }
}
