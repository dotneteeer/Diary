using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Diary.DAL.Migrations
{
    /// <inheritdoc />
    public partial class IndexToCreatedAtColumnAtReportTableAdded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Role",
                columns: new[] { "Id", "Name" },
                values: new object[] { 4L, "Api" });


            migrationBuilder.CreateIndex(
                name: "IX_Report_CreatedAt",
                table: "Report",
                column: "CreatedAt",
                descending: new bool[0]);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Report_CreatedAt",
                table: "Report");

            migrationBuilder.DeleteData(
                table: "Role",
                keyColumn: "Id",
                keyValue: 4L);
        }
    }
}
