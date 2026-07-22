using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace codeHappy.Data.Migrations
{
    /// <inheritdoc />
    public partial class RemoveShareUnusedFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Shares_SnippetId_SharedWith",
                table: "Shares");

            migrationBuilder.DropColumn(
                name: "Permission",
                table: "Shares");

            migrationBuilder.DropColumn(
                name: "SharedWith",
                table: "Shares");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "Shares");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Permission",
                table: "Shares",
                type: "text",
                nullable: false,
                defaultValue: "Viewer");

            migrationBuilder.AddColumn<string>(
                name: "SharedWith",
                table: "Shares",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "Shares",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "now()");

            migrationBuilder.CreateIndex(
                name: "IX_Shares_SnippetId_SharedWith",
                table: "Shares",
                columns: new[] { "SnippetId", "SharedWith" });
        }
    }
}
