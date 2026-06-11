using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Threddit.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AllowNullableContentAndTitleOnPost : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Post_Posts_SubThreadId_Title",
                schema: "threddit",
                table: "Post_Posts");

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                schema: "threddit",
                table: "Post_Posts",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "Content",
                schema: "threddit",
                table: "Post_Posts",
                type: "nvarchar(3000)",
                maxLength: 3000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(3000)",
                oldMaxLength: 3000);

            migrationBuilder.CreateIndex(
                name: "IX_Post_Posts_SubThreadId_Title",
                schema: "threddit",
                table: "Post_Posts",
                columns: new[] { "SubThreadId", "Title" },
                unique: true,
                filter: "[Title] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Post_Posts_SubThreadId_Title",
                schema: "threddit",
                table: "Post_Posts");

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                schema: "threddit",
                table: "Post_Posts",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Content",
                schema: "threddit",
                table: "Post_Posts",
                type: "nvarchar(3000)",
                maxLength: 3000,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(3000)",
                oldMaxLength: 3000,
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Post_Posts_SubThreadId_Title",
                schema: "threddit",
                table: "Post_Posts",
                columns: new[] { "SubThreadId", "Title" },
                unique: true);
        }
    }
}
