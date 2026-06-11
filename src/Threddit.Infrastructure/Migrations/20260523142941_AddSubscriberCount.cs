using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Threddit.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddSubscriberCount : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SubscriberCount",
                schema: "threddit",
                table: "SubThread_SubThreads",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_SubThread_SubThreads_SubscriberCount",
                schema: "threddit",
                table: "SubThread_SubThreads",
                column: "SubscriberCount");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_SubThread_SubThreads_SubscriberCount",
                schema: "threddit",
                table: "SubThread_SubThreads");

            migrationBuilder.DropColumn(
                name: "SubscriberCount",
                schema: "threddit",
                table: "SubThread_SubThreads");
        }
    }
}
