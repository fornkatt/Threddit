using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Threddit.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddScoreColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CommentScore",
                schema: "threddit",
                table: "User_Users",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "PostScore",
                schema: "threddit",
                table: "User_Users",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TotalScore",
                schema: "threddit",
                table: "User_Users",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Score",
                schema: "threddit",
                table: "Post_Posts",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Score",
                schema: "threddit",
                table: "Comment_Comments",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CommentScore",
                schema: "threddit",
                table: "User_Users");

            migrationBuilder.DropColumn(
                name: "PostScore",
                schema: "threddit",
                table: "User_Users");

            migrationBuilder.DropColumn(
                name: "TotalScore",
                schema: "threddit",
                table: "User_Users");

            migrationBuilder.DropColumn(
                name: "Score",
                schema: "threddit",
                table: "Post_Posts");

            migrationBuilder.DropColumn(
                name: "Score",
                schema: "threddit",
                table: "Comment_Comments");
        }
    }
}
