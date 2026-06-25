using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace codeHappy.Data.Migrations
{
    /// <inheritdoc />
    public partial class AdjustCascadeDeleteBehaviors : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Comments_Profiles_OwnerId",
                table: "Comments");

            migrationBuilder.DropForeignKey(
                name: "FK_Groups_Profiles_OwnerId",
                table: "Groups");

            migrationBuilder.DropForeignKey(
                name: "FK_Groups_Spaces_SpaceId",
                table: "Groups");

            migrationBuilder.DropForeignKey(
                name: "FK_Shares_Profiles_SharedBy",
                table: "Shares");

            migrationBuilder.DropForeignKey(
                name: "FK_Snippets_Groups_GroupId",
                table: "Snippets");

            migrationBuilder.DropForeignKey(
                name: "FK_Snippets_Profiles_OwnerId",
                table: "Snippets");

            migrationBuilder.DropForeignKey(
                name: "FK_Snippets_Spaces_SpaceId",
                table: "Snippets");

            migrationBuilder.DropIndex(
                name: "IX_Groups_OwnerId",
                table: "Groups");

            migrationBuilder.DropColumn(
                name: "OwnerId",
                table: "Groups");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Group_Position",
                table: "Groups",
                sql: "\"Position\" >= 0");

            migrationBuilder.AddForeignKey(
                name: "FK_Comments_Profiles_OwnerId",
                table: "Comments",
                column: "OwnerId",
                principalTable: "Profiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Groups_Spaces_SpaceId",
                table: "Groups",
                column: "SpaceId",
                principalTable: "Spaces",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Shares_Profiles_SharedBy",
                table: "Shares",
                column: "SharedBy",
                principalTable: "Profiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Snippets_Groups_GroupId",
                table: "Snippets",
                column: "GroupId",
                principalTable: "Groups",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Snippets_Profiles_OwnerId",
                table: "Snippets",
                column: "OwnerId",
                principalTable: "Profiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Snippets_Spaces_SpaceId",
                table: "Snippets",
                column: "SpaceId",
                principalTable: "Spaces",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Comments_Profiles_OwnerId",
                table: "Comments");

            migrationBuilder.DropForeignKey(
                name: "FK_Groups_Spaces_SpaceId",
                table: "Groups");

            migrationBuilder.DropForeignKey(
                name: "FK_Shares_Profiles_SharedBy",
                table: "Shares");

            migrationBuilder.DropForeignKey(
                name: "FK_Snippets_Groups_GroupId",
                table: "Snippets");

            migrationBuilder.DropForeignKey(
                name: "FK_Snippets_Profiles_OwnerId",
                table: "Snippets");

            migrationBuilder.DropForeignKey(
                name: "FK_Snippets_Spaces_SpaceId",
                table: "Snippets");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Group_Position",
                table: "Groups");

            migrationBuilder.AddColumn<Guid>(
                name: "OwnerId",
                table: "Groups",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_Groups_OwnerId",
                table: "Groups",
                column: "OwnerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Comments_Profiles_OwnerId",
                table: "Comments",
                column: "OwnerId",
                principalTable: "Profiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Groups_Profiles_OwnerId",
                table: "Groups",
                column: "OwnerId",
                principalTable: "Profiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Groups_Spaces_SpaceId",
                table: "Groups",
                column: "SpaceId",
                principalTable: "Spaces",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Shares_Profiles_SharedBy",
                table: "Shares",
                column: "SharedBy",
                principalTable: "Profiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Snippets_Groups_GroupId",
                table: "Snippets",
                column: "GroupId",
                principalTable: "Groups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Snippets_Profiles_OwnerId",
                table: "Snippets",
                column: "OwnerId",
                principalTable: "Profiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Snippets_Spaces_SpaceId",
                table: "Snippets",
                column: "SpaceId",
                principalTable: "Spaces",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
