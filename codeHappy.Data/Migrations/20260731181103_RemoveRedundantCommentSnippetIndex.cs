using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace codeHappy.Data.Migrations
{
    /// <inheritdoc />
    public partial class RemoveRedundantCommentSnippetIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Comments_SnippetId",
                table: "Comments");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Comments_SnippetId",
                table: "Comments",
                column: "SnippetId");
        }
    }
}
