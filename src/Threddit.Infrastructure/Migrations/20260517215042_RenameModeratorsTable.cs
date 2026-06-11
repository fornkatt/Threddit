using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Threddit.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RenameModeratorsTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_User_Moderators_SubThread_SubThreads_SubThreadId",
                schema: "threddit",
                table: "User_Moderators");

            migrationBuilder.DropForeignKey(
                name: "FK_User_Moderators_User_Users_UserId",
                schema: "threddit",
                table: "User_Moderators");

            migrationBuilder.DropPrimaryKey(
                name: "PK_User_Moderators",
                schema: "threddit",
                table: "User_Moderators");

            migrationBuilder.RenameTable(
                name: "User_Moderators",
                schema: "threddit",
                newName: "User_SubThreadModerators",
                newSchema: "threddit");

            migrationBuilder.RenameIndex(
                name: "IX_User_Moderators_UserId_SubThreadId",
                schema: "threddit",
                table: "User_SubThreadModerators",
                newName: "IX_User_SubThreadModerators_UserId_SubThreadId");

            migrationBuilder.RenameIndex(
                name: "IX_User_Moderators_SubThreadId",
                schema: "threddit",
                table: "User_SubThreadModerators",
                newName: "IX_User_SubThreadModerators_SubThreadId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_User_SubThreadModerators",
                schema: "threddit",
                table: "User_SubThreadModerators",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_User_SubThreadModerators_SubThread_SubThreads_SubThreadId",
                schema: "threddit",
                table: "User_SubThreadModerators",
                column: "SubThreadId",
                principalSchema: "threddit",
                principalTable: "SubThread_SubThreads",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_User_SubThreadModerators_User_Users_UserId",
                schema: "threddit",
                table: "User_SubThreadModerators",
                column: "UserId",
                principalSchema: "threddit",
                principalTable: "User_Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_User_SubThreadModerators_SubThread_SubThreads_SubThreadId",
                schema: "threddit",
                table: "User_SubThreadModerators");

            migrationBuilder.DropForeignKey(
                name: "FK_User_SubThreadModerators_User_Users_UserId",
                schema: "threddit",
                table: "User_SubThreadModerators");

            migrationBuilder.DropPrimaryKey(
                name: "PK_User_SubThreadModerators",
                schema: "threddit",
                table: "User_SubThreadModerators");

            migrationBuilder.RenameTable(
                name: "User_SubThreadModerators",
                schema: "threddit",
                newName: "User_Moderators",
                newSchema: "threddit");

            migrationBuilder.RenameIndex(
                name: "IX_User_SubThreadModerators_UserId_SubThreadId",
                schema: "threddit",
                table: "User_Moderators",
                newName: "IX_User_Moderators_UserId_SubThreadId");

            migrationBuilder.RenameIndex(
                name: "IX_User_SubThreadModerators_SubThreadId",
                schema: "threddit",
                table: "User_Moderators",
                newName: "IX_User_Moderators_SubThreadId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_User_Moderators",
                schema: "threddit",
                table: "User_Moderators",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_User_Moderators_SubThread_SubThreads_SubThreadId",
                schema: "threddit",
                table: "User_Moderators",
                column: "SubThreadId",
                principalSchema: "threddit",
                principalTable: "SubThread_SubThreads",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_User_Moderators_User_Users_UserId",
                schema: "threddit",
                table: "User_Moderators",
                column: "UserId",
                principalSchema: "threddit",
                principalTable: "User_Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
