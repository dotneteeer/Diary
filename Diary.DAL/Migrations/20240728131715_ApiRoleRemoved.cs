using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Diary.DAL.Migrations
{
    /// <inheritdoc />
    public partial class ApiRoleRemoved : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Role",
                keyColumn: "Id",
                keyValue: 4L);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Role",
                columns: new[] { "Id", "Name" },
                values: new object[] { 4L, "Api" });
        }
    }
}
