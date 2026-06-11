using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Threddit.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AllowPostSlugNull : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Post_Posts_SubThreadId_Slug",
                schema: "threddit",
                table: "Post_Posts");

            migrationBuilder.AlterColumn<string>(
                name: "Slug",
                schema: "threddit",
                table: "Post_Posts",
                type: "nvarchar(120)",
                maxLength: 120,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(120)",
                oldMaxLength: 120);

            migrationBuilder.CreateIndex(
                name: "IX_Post_Posts_SubThreadId_Slug",
                schema: "threddit",
                table: "Post_Posts",
                columns: new[] { "SubThreadId", "Slug" },
                unique: true,
                filter: "[Slug] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Post_Posts_SubThreadId_Slug",
                schema: "threddit",
                table: "Post_Posts");

            migrationBuilder.AlterColumn<string>(
                name: "Slug",
                schema: "threddit",
                table: "Post_Posts",
                type: "nvarchar(120)",
                maxLength: 120,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(120)",
                oldMaxLength: 120,
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Post_Posts_SubThreadId_Slug",
                schema: "threddit",
                table: "Post_Posts",
                columns: new[] { "SubThreadId", "Slug" },
                unique: true);
        }
    }
}
