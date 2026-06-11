using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Threddit.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddScoreIndices : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_User_Users_TotalScore",
                schema: "threddit",
                table: "User_Users",
                column: "TotalScore");

            migrationBuilder.CreateIndex(
                name: "IX_Post_Posts_Score",
                schema: "threddit",
                table: "Post_Posts",
                column: "Score");

            migrationBuilder.CreateIndex(
                name: "IX_Comment_Comments_Score",
                schema: "threddit",
                table: "Comment_Comments",
                column: "Score");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_User_Users_TotalScore",
                schema: "threddit",
                table: "User_Users");

            migrationBuilder.DropIndex(
                name: "IX_Post_Posts_Score",
                schema: "threddit",
                table: "Post_Posts");

            migrationBuilder.DropIndex(
                name: "IX_Comment_Comments_Score",
                schema: "threddit",
                table: "Comment_Comments");
        }
    }
}
