using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Diary.DAL.Migrations
{
    /// <inheritdoc />
    public partial class ValueObjectNameWasAddedAndNameReplacedWithFullName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Name",
                table: "Report",
                newName: "FullName");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "FullName",
                table: "Report",
                newName: "Name");
        }
    }
}
