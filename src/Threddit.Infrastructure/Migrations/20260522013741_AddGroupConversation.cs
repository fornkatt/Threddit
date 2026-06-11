using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Threddit.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddGroupConversation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsRead",
                schema: "threddit",
                table: "Conversation_DirectMessages");

            migrationBuilder.AddColumn<int>(
                name: "CommentCount",
                schema: "threddit",
                table: "Post_Posts",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<Guid>(
                name: "ConversationId",
                schema: "threddit",
                table: "Conversation_DirectMessages",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddColumn<Guid>(
                name: "GroupConversationId",
                schema: "threddit",
                table: "Conversation_DirectMessages",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Conversation_DirectMessageReads",
                schema: "threddit",
                columns: table => new
                {
                    DirectMessageId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ReadAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Conversation_DirectMessageReads", x => new { x.DirectMessageId, x.UserId });
                    table.ForeignKey(
                        name: "FK_Conversation_DirectMessageReads_Conversation_DirectMessages_DirectMessageId",
                        column: x => x.DirectMessageId,
                        principalSchema: "threddit",
                        principalTable: "Conversation_DirectMessages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Conversation_DirectMessageReads_User_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "threddit",
                        principalTable: "User_Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Conversation_GroupConversations",
                schema: "threddit",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CreatedById = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Conversation_GroupConversations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Conversation_GroupConversations_User_Users_CreatedById",
                        column: x => x.CreatedById,
                        principalSchema: "threddit",
                        principalTable: "User_Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Conversation_GroupConversationMembers",
                schema: "threddit",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    GroupConversationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    JoinedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LeftAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Conversation_GroupConversationMembers", x => new { x.UserId, x.GroupConversationId, x.JoinedAt });
                    table.ForeignKey(
                        name: "FK_Conversation_GroupConversationMembers_Conversation_GroupConversations_GroupConversationId",
                        column: x => x.GroupConversationId,
                        principalSchema: "threddit",
                        principalTable: "Conversation_GroupConversations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Conversation_GroupConversationMembers_User_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "threddit",
                        principalTable: "User_Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Post_Posts_CommentCount",
                schema: "threddit",
                table: "Post_Posts",
                column: "CommentCount");

            migrationBuilder.CreateIndex(
                name: "IX_Conversation_DirectMessages_GroupConversationId",
                schema: "threddit",
                table: "Conversation_DirectMessages",
                column: "GroupConversationId");

            migrationBuilder.CreateIndex(
                name: "IX_Conversation_DirectMessageReads_UserId",
                schema: "threddit",
                table: "Conversation_DirectMessageReads",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Conversation_GroupConversationMembers_GroupConversationId",
                schema: "threddit",
                table: "Conversation_GroupConversationMembers",
                column: "GroupConversationId");

            migrationBuilder.CreateIndex(
                name: "IX_Conversation_GroupConversations_CreatedById",
                schema: "threddit",
                table: "Conversation_GroupConversations",
                column: "CreatedById");

            migrationBuilder.AddForeignKey(
                name: "FK_Conversation_DirectMessages_Conversation_GroupConversations_GroupConversationId",
                schema: "threddit",
                table: "Conversation_DirectMessages",
                column: "GroupConversationId",
                principalSchema: "threddit",
                principalTable: "Conversation_GroupConversations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Conversation_DirectMessages_Conversation_GroupConversations_GroupConversationId",
                schema: "threddit",
                table: "Conversation_DirectMessages");

            migrationBuilder.DropTable(
                name: "Conversation_DirectMessageReads",
                schema: "threddit");

            migrationBuilder.DropTable(
                name: "Conversation_GroupConversationMembers",
                schema: "threddit");

            migrationBuilder.DropTable(
                name: "Conversation_GroupConversations",
                schema: "threddit");

            migrationBuilder.DropIndex(
                name: "IX_Post_Posts_CommentCount",
                schema: "threddit",
                table: "Post_Posts");

            migrationBuilder.DropIndex(
                name: "IX_Conversation_DirectMessages_GroupConversationId",
                schema: "threddit",
                table: "Conversation_DirectMessages");

            migrationBuilder.DropColumn(
                name: "CommentCount",
                schema: "threddit",
                table: "Post_Posts");

            migrationBuilder.DropColumn(
                name: "GroupConversationId",
                schema: "threddit",
                table: "Conversation_DirectMessages");

            migrationBuilder.AlterColumn<Guid>(
                name: "ConversationId",
                schema: "threddit",
                table: "Conversation_DirectMessages",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsRead",
                schema: "threddit",
                table: "Conversation_DirectMessages",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
