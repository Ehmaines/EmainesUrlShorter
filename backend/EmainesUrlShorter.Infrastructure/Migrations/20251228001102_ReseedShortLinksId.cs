using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EmainesUrlShorter.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ReseedShortLinksId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DBCC CHECKIDENT ('ShortLinks', RESEED, 14000000);");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
