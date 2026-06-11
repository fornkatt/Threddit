using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Threddit.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FixVoteFKs : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Comment_CommentVotes_Comment_Comments_CommentId",
                schema: "threddit",
                table: "Comment_CommentVotes");

            migrationBuilder.DropForeignKey(
                name: "FK_Post_PostVotes_Post_Posts_PostId",
                schema: "threddit",
                table: "Post_PostVotes");

            migrationBuilder.DropIndex(
                name: "IX_Post_PostVotes_PostId_UserId",
                schema: "threddit",
                table: "Post_PostVotes");

            migrationBuilder.DropIndex(
                name: "IX_Comment_CommentVotes_UserId_CommentId",
                schema: "threddit",
                table: "Comment_CommentVotes");

            migrationBuilder.AlterColumn<Guid>(
                name: "PostId",
                schema: "threddit",
                table: "Post_PostVotes",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "CommentId",
                schema: "threddit",
                table: "Comment_CommentVotes",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Post_PostVotes_PostId_UserId",
                schema: "threddit",
                table: "Post_PostVotes",
                columns: new[] { "PostId", "UserId" },
                unique: true,
                filter: "[UserId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Comment_CommentVotes_UserId_CommentId",
                schema: "threddit",
                table: "Comment_CommentVotes",
                columns: new[] { "UserId", "CommentId" },
                unique: true,
                filter: "[UserId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_Comment_CommentVotes_Comment_Comments_CommentId",
                schema: "threddit",
                table: "Comment_CommentVotes",
                column: "CommentId",
                principalSchema: "threddit",
                principalTable: "Comment_Comments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Post_PostVotes_Post_Posts_PostId",
                schema: "threddit",
                table: "Post_PostVotes",
                column: "PostId",
                principalSchema: "threddit",
                principalTable: "Post_Posts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Comment_CommentVotes_Comment_Comments_CommentId",
                schema: "threddit",
                table: "Comment_CommentVotes");

            migrationBuilder.DropForeignKey(
                name: "FK_Post_PostVotes_Post_Posts_PostId",
                schema: "threddit",
                table: "Post_PostVotes");

            migrationBuilder.DropIndex(
                name: "IX_Post_PostVotes_PostId_UserId",
                schema: "threddit",
                table: "Post_PostVotes");

            migrationBuilder.DropIndex(
                name: "IX_Comment_CommentVotes_UserId_CommentId",
                schema: "threddit",
                table: "Comment_CommentVotes");

            migrationBuilder.AlterColumn<Guid>(
                name: "PostId",
                schema: "threddit",
                table: "Post_PostVotes",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<Guid>(
                name: "CommentId",
                schema: "threddit",
                table: "Comment_CommentVotes",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.CreateIndex(
                name: "IX_Post_PostVotes_PostId_UserId",
                schema: "threddit",
                table: "Post_PostVotes",
                columns: new[] { "PostId", "UserId" },
                unique: true,
                filter: "[PostId] IS NOT NULL AND [UserId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Comment_CommentVotes_UserId_CommentId",
                schema: "threddit",
                table: "Comment_CommentVotes",
                columns: new[] { "UserId", "CommentId" },
                unique: true,
                filter: "[UserId] IS NOT NULL AND [CommentId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_Comment_CommentVotes_Comment_Comments_CommentId",
                schema: "threddit",
                table: "Comment_CommentVotes",
                column: "CommentId",
                principalSchema: "threddit",
                principalTable: "Comment_Comments",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Post_PostVotes_Post_Posts_PostId",
                schema: "threddit",
                table: "Post_PostVotes",
                column: "PostId",
                principalSchema: "threddit",
                principalTable: "Post_Posts",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }
    }
}
