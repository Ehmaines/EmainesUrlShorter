using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EmainesUrlShorter.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateTableStructures : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LinkAccesses_ShortLinks_ShortLinkId",
                table: "LinkAccesses");

            migrationBuilder.RenameColumn(
                name: "ShortLinkId",
                table: "LinkAccesses",
                newName: "LinkId");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "LinkAccesses",
                newName: "ClickedAt");

            migrationBuilder.RenameIndex(
                name: "IX_LinkAccesses_ShortLinkId",
                table: "LinkAccesses",
                newName: "IX_LinkAccesses_LinkId");

            migrationBuilder.AddColumn<int>(
                name: "TotalClicks",
                table: "ShortLinks",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Browser",
                table: "LinkAccesses",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Country",
                table: "LinkAccesses",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Device",
                table: "LinkAccesses",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddForeignKey(
                name: "FK_LinkAccesses_ShortLinks_LinkId",
                table: "LinkAccesses",
                column: "LinkId",
                principalTable: "ShortLinks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LinkAccesses_ShortLinks_LinkId",
                table: "LinkAccesses");

            migrationBuilder.DropColumn(
                name: "TotalClicks",
                table: "ShortLinks");

            migrationBuilder.DropColumn(
                name: "Browser",
                table: "LinkAccesses");

            migrationBuilder.DropColumn(
                name: "Country",
                table: "LinkAccesses");

            migrationBuilder.DropColumn(
                name: "Device",
                table: "LinkAccesses");

            migrationBuilder.RenameColumn(
                name: "LinkId",
                table: "LinkAccesses",
                newName: "ShortLinkId");

            migrationBuilder.RenameColumn(
                name: "ClickedAt",
                table: "LinkAccesses",
                newName: "CreatedAt");

            migrationBuilder.RenameIndex(
                name: "IX_LinkAccesses_LinkId",
                table: "LinkAccesses",
                newName: "IX_LinkAccesses_ShortLinkId");

            migrationBuilder.AddForeignKey(
                name: "FK_LinkAccesses_ShortLinks_ShortLinkId",
                table: "LinkAccesses",
                column: "ShortLinkId",
                principalTable: "ShortLinks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
