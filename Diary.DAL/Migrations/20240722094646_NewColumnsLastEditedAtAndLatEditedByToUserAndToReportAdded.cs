using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Diary.DAL.Migrations
{
    /// <inheritdoc />
    public partial class NewColumnsLastEditedAtAndLatEditedByToUserAndToReportAdded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.AddColumn<DateTime>(
                name: "LastEditedAt",
                table: "User",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<long>(
                name: "LastEditedBy",
                table: "User",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastEditedAt",
                table: "Report",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<long>(
                name: "LastEditedBy",
                table: "Report",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateIndex(
                name: "IX_Report_LastEditedAt",
                table: "Report",
                column: "LastEditedAt",
                descending: new bool[0]);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Report_LastEditedAt",
                table: "Report");

            migrationBuilder.DropColumn(
                name: "LastEditedAt",
                table: "User");

            migrationBuilder.DropColumn(
                name: "LastEditedBy",
                table: "User");

            migrationBuilder.DropColumn(
                name: "LastEditedAt",
                table: "Report");

            migrationBuilder.DropColumn(
                name: "LastEditedBy",
                table: "Report");
        }
    }
}
