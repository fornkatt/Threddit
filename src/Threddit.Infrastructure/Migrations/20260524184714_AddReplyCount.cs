using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Threddit.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddReplyCount : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ReplyCount",
                schema: "threddit",
                table: "Comment_Comments",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Comment_Comments_ReplyCount",
                schema: "threddit",
                table: "Comment_Comments",
                column: "ReplyCount");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Comment_Comments_ReplyCount",
                schema: "threddit",
                table: "Comment_Comments");

            migrationBuilder.DropColumn(
                name: "ReplyCount",
                schema: "threddit",
                table: "Comment_Comments");
        }
    }
}
