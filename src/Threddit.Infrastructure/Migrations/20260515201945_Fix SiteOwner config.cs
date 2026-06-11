using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Threddit.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FixSiteOwnerconfig : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SiteOwner_User_Users_UserId",
                schema: "threddit",
                table: "SiteOwner");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SiteOwner",
                schema: "threddit",
                table: "SiteOwner");

            migrationBuilder.RenameTable(
                name: "SiteOwner",
                schema: "threddit",
                newName: "User_SiteOwner",
                newSchema: "threddit");

            migrationBuilder.RenameIndex(
                name: "IX_SiteOwner_UserId",
                schema: "threddit",
                table: "User_SiteOwner",
                newName: "IX_User_SiteOwner_UserId");

            migrationBuilder.AlterColumn<int>(
                name: "SingletonKey",
                schema: "threddit",
                table: "User_SiteOwner",
                type: "int",
                nullable: false,
                defaultValue: 1,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddPrimaryKey(
                name: "PK_User_SiteOwner",
                schema: "threddit",
                table: "User_SiteOwner",
                column: "SingletonKey");

            migrationBuilder.AddCheckConstraint(
                name: "CK_SiteOwner_SingletonKey",
                schema: "threddit",
                table: "User_SiteOwner",
                sql: "[SingletonKey] = 1");

            migrationBuilder.AddForeignKey(
                name: "FK_User_SiteOwner_User_Users_UserId",
                schema: "threddit",
                table: "User_SiteOwner",
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
                name: "FK_User_SiteOwner_User_Users_UserId",
                schema: "threddit",
                table: "User_SiteOwner");

            migrationBuilder.DropPrimaryKey(
                name: "PK_User_SiteOwner",
                schema: "threddit",
                table: "User_SiteOwner");

            migrationBuilder.DropCheckConstraint(
                name: "CK_SiteOwner_SingletonKey",
                schema: "threddit",
                table: "User_SiteOwner");

            migrationBuilder.RenameTable(
                name: "User_SiteOwner",
                schema: "threddit",
                newName: "SiteOwner",
                newSchema: "threddit");

            migrationBuilder.RenameIndex(
                name: "IX_User_SiteOwner_UserId",
                schema: "threddit",
                table: "SiteOwner",
                newName: "IX_SiteOwner_UserId");

            migrationBuilder.AlterColumn<int>(
                name: "SingletonKey",
                schema: "threddit",
                table: "SiteOwner",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldDefaultValue: 1);

            migrationBuilder.AddPrimaryKey(
                name: "PK_SiteOwner",
                schema: "threddit",
                table: "SiteOwner",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_SiteOwner_User_Users_UserId",
                schema: "threddit",
                table: "SiteOwner",
                column: "UserId",
                principalSchema: "threddit",
                principalTable: "User_Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
