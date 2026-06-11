using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Threddit.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Finetunedentities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Comment_Comments_User_Users_UserId",
                schema: "threddit",
                table: "Comment_Comments");

            migrationBuilder.DropForeignKey(
                name: "FK_Comment_CommentVotes_User_Users_UserId",
                schema: "threddit",
                table: "Comment_CommentVotes");

            migrationBuilder.DropForeignKey(
                name: "FK_Conversation_Conversations_User_Users_InitiatorId",
                schema: "threddit",
                table: "Conversation_Conversations");

            migrationBuilder.DropForeignKey(
                name: "FK_Conversation_Conversations_User_Users_RecipientId",
                schema: "threddit",
                table: "Conversation_Conversations");

            migrationBuilder.DropForeignKey(
                name: "FK_Conversation_DirectMessages_User_Users_SenderId",
                schema: "threddit",
                table: "Conversation_DirectMessages");

            migrationBuilder.DropForeignKey(
                name: "FK_Post_PinnedSubThreadPosts_SubThread_SubThreads_SubThreadId",
                schema: "threddit",
                table: "Post_PinnedSubThreadPosts");

            migrationBuilder.DropForeignKey(
                name: "FK_Post_PinnedSubThreadPosts_User_Users_PinnedById",
                schema: "threddit",
                table: "Post_PinnedSubThreadPosts");

            migrationBuilder.DropForeignKey(
                name: "FK_Post_Posts_User_Users_UserId",
                schema: "threddit",
                table: "Post_Posts");

            migrationBuilder.DropForeignKey(
                name: "FK_Post_PostVotes_User_Users_UserId",
                schema: "threddit",
                table: "Post_PostVotes");

            migrationBuilder.DropForeignKey(
                name: "FK_SubThread_SubThreadRules_User_Users_CreatedById",
                schema: "threddit",
                table: "SubThread_SubThreadRules");

            migrationBuilder.DropForeignKey(
                name: "FK_SubThread_SubThreadRules_User_Users_LastUpdatedById",
                schema: "threddit",
                table: "SubThread_SubThreadRules");

            migrationBuilder.DropForeignKey(
                name: "FK_SubThread_SubThreads_User_Users_CreatedById",
                schema: "threddit",
                table: "SubThread_SubThreads");

            migrationBuilder.DropTable(
                name: "User_BannedUsers",
                schema: "threddit");

            migrationBuilder.DropIndex(
                name: "IX_User_FollowedUsers_FollowerId_FollowedId",
                schema: "threddit",
                table: "User_FollowedUsers");

            migrationBuilder.DropIndex(
                name: "IX_User_BlockedUsers_BlockerId_BlockedId",
                schema: "threddit",
                table: "User_BlockedUsers");

            migrationBuilder.DropIndex(
                name: "IX_Post_Posts_SubThreadId",
                schema: "threddit",
                table: "Post_Posts");

            migrationBuilder.DropColumn(
                name: "Rule",
                schema: "threddit",
                table: "SubThread_SubThreadRules");

            migrationBuilder.RenameColumn(
                name: "UserId",
                schema: "threddit",
                table: "Post_Posts",
                newName: "PostedById");

            migrationBuilder.RenameIndex(
                name: "IX_Post_Posts_UserId",
                schema: "threddit",
                table: "Post_Posts",
                newName: "IX_Post_Posts_PostedById");

            migrationBuilder.RenameColumn(
                name: "UserId",
                schema: "threddit",
                table: "Comment_Comments",
                newName: "CommentedById");

            migrationBuilder.RenameIndex(
                name: "IX_Comment_Comments_UserId",
                schema: "threddit",
                table: "Comment_Comments",
                newName: "IX_Comment_Comments_CommentedById");

            migrationBuilder.AddColumn<int>(
                name: "Permissions",
                schema: "threddit",
                table: "User_Moderators",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<Guid>(
                name: "FollowerId",
                schema: "threddit",
                table: "User_FollowedUsers",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<Guid>(
                name: "FollowedId",
                schema: "threddit",
                table: "User_FollowedUsers",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<Guid>(
                name: "BlockerId",
                schema: "threddit",
                table: "User_BlockedUsers",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<Guid>(
                name: "BlockedId",
                schema: "threddit",
                table: "User_BlockedUsers",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddColumn<string>(
                name: "RuleContent",
                schema: "threddit",
                table: "SubThread_SubThreadRules",
                type: "nvarchar(300)",
                maxLength: 300,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "RuleTitle",
                schema: "threddit",
                table: "SubThread_SubThreadRules",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "IsDownvote",
                schema: "threddit",
                table: "Post_PostVotes",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                schema: "threddit",
                table: "Post_Posts",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                schema: "threddit",
                table: "Post_Posts",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Slug",
                schema: "threddit",
                table: "Post_Posts",
                type: "nvarchar(120)",
                maxLength: 120,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<Guid>(
                name: "SubThreadId",
                schema: "threddit",
                table: "Post_PinnedSubThreadPosts",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsReply",
                schema: "threddit",
                table: "Conversation_DirectMessages",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<Guid>(
                name: "ParentMessageId",
                schema: "threddit",
                table: "Conversation_DirectMessages",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Report_Reports",
                schema: "threddit",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Category = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Type = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Message = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    ReporterId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    SubThreadId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    TargetPostId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    PostId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    TargetCommentId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    TargetUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    TargetSubThreadId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    TargetDirectMessageId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ReportedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Report_Reports", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Report_Reports_Comment_Comments_TargetCommentId",
                        column: x => x.TargetCommentId,
                        principalSchema: "threddit",
                        principalTable: "Comment_Comments",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Report_Reports_Conversation_DirectMessages_TargetDirectMessageId",
                        column: x => x.TargetDirectMessageId,
                        principalSchema: "threddit",
                        principalTable: "Conversation_DirectMessages",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Report_Reports_Post_Posts_PostId",
                        column: x => x.PostId,
                        principalSchema: "threddit",
                        principalTable: "Post_Posts",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Report_Reports_Post_Posts_TargetPostId",
                        column: x => x.TargetPostId,
                        principalSchema: "threddit",
                        principalTable: "Post_Posts",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Report_Reports_SubThread_SubThreads_SubThreadId",
                        column: x => x.SubThreadId,
                        principalSchema: "threddit",
                        principalTable: "SubThread_SubThreads",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Report_Reports_SubThread_SubThreads_TargetSubThreadId",
                        column: x => x.TargetSubThreadId,
                        principalSchema: "threddit",
                        principalTable: "SubThread_SubThreads",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Report_Reports_User_Users_ReporterId",
                        column: x => x.ReporterId,
                        principalSchema: "threddit",
                        principalTable: "User_Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Report_Reports_User_Users_TargetUserId",
                        column: x => x.TargetUserId,
                        principalSchema: "threddit",
                        principalTable: "User_Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SiteOwner",
                schema: "threddit",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SingletonKey = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AssignedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SiteOwner", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SiteOwner_User_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "threddit",
                        principalTable: "User_Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "User_BannedSiteUsers",
                schema: "threddit",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Reason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BannedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ExpiresAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    BannedById = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    LastEditedById = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    EditedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_User_BannedSiteUsers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_User_BannedSiteUsers_User_Users_BannedById",
                        column: x => x.BannedById,
                        principalSchema: "threddit",
                        principalTable: "User_Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_User_BannedSiteUsers_User_Users_LastEditedById",
                        column: x => x.LastEditedById,
                        principalSchema: "threddit",
                        principalTable: "User_Users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_User_BannedSiteUsers_User_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "threddit",
                        principalTable: "User_Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "User_BannedSubThreadUsers",
                schema: "threddit",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Reason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SubThreadId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BannedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ExpiresAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EditedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    BannedById = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    LastEditedById = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_User_BannedSubThreadUsers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_User_BannedSubThreadUsers_SubThread_SubThreads_SubThreadId",
                        column: x => x.SubThreadId,
                        principalSchema: "threddit",
                        principalTable: "SubThread_SubThreads",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_User_BannedSubThreadUsers_User_Users_BannedById",
                        column: x => x.BannedById,
                        principalSchema: "threddit",
                        principalTable: "User_Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_User_BannedSubThreadUsers_User_Users_LastEditedById",
                        column: x => x.LastEditedById,
                        principalSchema: "threddit",
                        principalTable: "User_Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_User_BannedSubThreadUsers_User_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "threddit",
                        principalTable: "User_Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_User_FollowedUsers_FollowerId_FollowedId",
                schema: "threddit",
                table: "User_FollowedUsers",
                columns: new[] { "FollowerId", "FollowedId" },
                unique: true,
                filter: "[FollowerId] IS NOT NULL AND [FollowedId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_User_BlockedUsers_BlockerId_BlockedId",
                schema: "threddit",
                table: "User_BlockedUsers",
                columns: new[] { "BlockerId", "BlockedId" },
                unique: true,
                filter: "[BlockerId] IS NOT NULL AND [BlockedId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Post_Posts_PostedAt",
                schema: "threddit",
                table: "Post_Posts",
                column: "PostedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Post_Posts_SubThreadId_Slug",
                schema: "threddit",
                table: "Post_Posts",
                columns: new[] { "SubThreadId", "Slug" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Post_Posts_SubThreadId_Title",
                schema: "threddit",
                table: "Post_Posts",
                columns: new[] { "SubThreadId", "Title" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Conversation_DirectMessages_ParentMessageId",
                schema: "threddit",
                table: "Conversation_DirectMessages",
                column: "ParentMessageId");

            migrationBuilder.CreateIndex(
                name: "IX_Comment_Comments_CommentedAt",
                schema: "threddit",
                table: "Comment_Comments",
                column: "CommentedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Report_AdminQueue",
                schema: "threddit",
                table: "Report_Reports",
                column: "Status",
                filter: "[SubThreadId] IS NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Report_Reports_PostId",
                schema: "threddit",
                table: "Report_Reports",
                column: "PostId");

            migrationBuilder.CreateIndex(
                name: "IX_Report_Reports_PostId_Status",
                schema: "threddit",
                table: "Report_Reports",
                columns: new[] { "PostId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_Report_Reports_ReportedAt",
                schema: "threddit",
                table: "Report_Reports",
                column: "ReportedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Report_Reports_ReporterId",
                schema: "threddit",
                table: "Report_Reports",
                column: "ReporterId");

            migrationBuilder.CreateIndex(
                name: "IX_Report_Reports_SubThreadId",
                schema: "threddit",
                table: "Report_Reports",
                column: "SubThreadId");

            migrationBuilder.CreateIndex(
                name: "IX_Report_Reports_SubThreadId_Status",
                schema: "threddit",
                table: "Report_Reports",
                columns: new[] { "SubThreadId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_Report_Reports_TargetCommentId",
                schema: "threddit",
                table: "Report_Reports",
                column: "TargetCommentId");

            migrationBuilder.CreateIndex(
                name: "IX_Report_Reports_TargetDirectMessageId",
                schema: "threddit",
                table: "Report_Reports",
                column: "TargetDirectMessageId");

            migrationBuilder.CreateIndex(
                name: "IX_Report_Reports_TargetPostId",
                schema: "threddit",
                table: "Report_Reports",
                column: "TargetPostId");

            migrationBuilder.CreateIndex(
                name: "IX_Report_Reports_TargetSubThreadId",
                schema: "threddit",
                table: "Report_Reports",
                column: "TargetSubThreadId");

            migrationBuilder.CreateIndex(
                name: "IX_Report_Reports_TargetUserId",
                schema: "threddit",
                table: "Report_Reports",
                column: "TargetUserId");

            migrationBuilder.CreateIndex(
                name: "IX_SiteOwner_UserId",
                schema: "threddit",
                table: "SiteOwner",
                column: "UserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_User_BannedSiteUsers_BannedById",
                schema: "threddit",
                table: "User_BannedSiteUsers",
                column: "BannedById");

            migrationBuilder.CreateIndex(
                name: "IX_User_BannedSiteUsers_LastEditedById",
                schema: "threddit",
                table: "User_BannedSiteUsers",
                column: "LastEditedById");

            migrationBuilder.CreateIndex(
                name: "IX_User_BannedSiteUsers_UserId",
                schema: "threddit",
                table: "User_BannedSiteUsers",
                column: "UserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_User_BannedSubThreadUsers_BannedById",
                schema: "threddit",
                table: "User_BannedSubThreadUsers",
                column: "BannedById");

            migrationBuilder.CreateIndex(
                name: "IX_User_BannedSubThreadUsers_LastEditedById",
                schema: "threddit",
                table: "User_BannedSubThreadUsers",
                column: "LastEditedById");

            migrationBuilder.CreateIndex(
                name: "IX_User_BannedSubThreadUsers_SubThreadId",
                schema: "threddit",
                table: "User_BannedSubThreadUsers",
                column: "SubThreadId");

            migrationBuilder.CreateIndex(
                name: "IX_User_BannedSubThreadUsers_UserId_SubThreadId",
                schema: "threddit",
                table: "User_BannedSubThreadUsers",
                columns: new[] { "UserId", "SubThreadId" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Comment_Comments_User_Users_CommentedById",
                schema: "threddit",
                table: "Comment_Comments",
                column: "CommentedById",
                principalSchema: "threddit",
                principalTable: "User_Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Comment_CommentVotes_User_Users_UserId",
                schema: "threddit",
                table: "Comment_CommentVotes",
                column: "UserId",
                principalSchema: "threddit",
                principalTable: "User_Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Conversation_Conversations_User_Users_InitiatorId",
                schema: "threddit",
                table: "Conversation_Conversations",
                column: "InitiatorId",
                principalSchema: "threddit",
                principalTable: "User_Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Conversation_Conversations_User_Users_RecipientId",
                schema: "threddit",
                table: "Conversation_Conversations",
                column: "RecipientId",
                principalSchema: "threddit",
                principalTable: "User_Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Conversation_DirectMessages_Conversation_DirectMessages_ParentMessageId",
                schema: "threddit",
                table: "Conversation_DirectMessages",
                column: "ParentMessageId",
                principalSchema: "threddit",
                principalTable: "Conversation_DirectMessages",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Conversation_DirectMessages_User_Users_SenderId",
                schema: "threddit",
                table: "Conversation_DirectMessages",
                column: "SenderId",
                principalSchema: "threddit",
                principalTable: "User_Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Post_PinnedSubThreadPosts_SubThread_SubThreads_SubThreadId",
                schema: "threddit",
                table: "Post_PinnedSubThreadPosts",
                column: "SubThreadId",
                principalSchema: "threddit",
                principalTable: "SubThread_SubThreads",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Post_PinnedSubThreadPosts_User_Users_PinnedById",
                schema: "threddit",
                table: "Post_PinnedSubThreadPosts",
                column: "PinnedById",
                principalSchema: "threddit",
                principalTable: "User_Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Post_Posts_User_Users_PostedById",
                schema: "threddit",
                table: "Post_Posts",
                column: "PostedById",
                principalSchema: "threddit",
                principalTable: "User_Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Post_PostVotes_User_Users_UserId",
                schema: "threddit",
                table: "Post_PostVotes",
                column: "UserId",
                principalSchema: "threddit",
                principalTable: "User_Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_SubThread_SubThreadRules_User_Users_CreatedById",
                schema: "threddit",
                table: "SubThread_SubThreadRules",
                column: "CreatedById",
                principalSchema: "threddit",
                principalTable: "User_Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_SubThread_SubThreadRules_User_Users_LastUpdatedById",
                schema: "threddit",
                table: "SubThread_SubThreadRules",
                column: "LastUpdatedById",
                principalSchema: "threddit",
                principalTable: "User_Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_SubThread_SubThreads_User_Users_CreatedById",
                schema: "threddit",
                table: "SubThread_SubThreads",
                column: "CreatedById",
                principalSchema: "threddit",
                principalTable: "User_Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Comment_Comments_User_Users_CommentedById",
                schema: "threddit",
                table: "Comment_Comments");

            migrationBuilder.DropForeignKey(
                name: "FK_Comment_CommentVotes_User_Users_UserId",
                schema: "threddit",
                table: "Comment_CommentVotes");

            migrationBuilder.DropForeignKey(
                name: "FK_Conversation_Conversations_User_Users_InitiatorId",
                schema: "threddit",
                table: "Conversation_Conversations");

            migrationBuilder.DropForeignKey(
                name: "FK_Conversation_Conversations_User_Users_RecipientId",
                schema: "threddit",
                table: "Conversation_Conversations");

            migrationBuilder.DropForeignKey(
                name: "FK_Conversation_DirectMessages_Conversation_DirectMessages_ParentMessageId",
                schema: "threddit",
                table: "Conversation_DirectMessages");

            migrationBuilder.DropForeignKey(
                name: "FK_Conversation_DirectMessages_User_Users_SenderId",
                schema: "threddit",
                table: "Conversation_DirectMessages");

            migrationBuilder.DropForeignKey(
                name: "FK_Post_PinnedSubThreadPosts_SubThread_SubThreads_SubThreadId",
                schema: "threddit",
                table: "Post_PinnedSubThreadPosts");

            migrationBuilder.DropForeignKey(
                name: "FK_Post_PinnedSubThreadPosts_User_Users_PinnedById",
                schema: "threddit",
                table: "Post_PinnedSubThreadPosts");

            migrationBuilder.DropForeignKey(
                name: "FK_Post_Posts_User_Users_PostedById",
                schema: "threddit",
                table: "Post_Posts");

            migrationBuilder.DropForeignKey(
                name: "FK_Post_PostVotes_User_Users_UserId",
                schema: "threddit",
                table: "Post_PostVotes");

            migrationBuilder.DropForeignKey(
                name: "FK_SubThread_SubThreadRules_User_Users_CreatedById",
                schema: "threddit",
                table: "SubThread_SubThreadRules");

            migrationBuilder.DropForeignKey(
                name: "FK_SubThread_SubThreadRules_User_Users_LastUpdatedById",
                schema: "threddit",
                table: "SubThread_SubThreadRules");

            migrationBuilder.DropForeignKey(
                name: "FK_SubThread_SubThreads_User_Users_CreatedById",
                schema: "threddit",
                table: "SubThread_SubThreads");

            migrationBuilder.DropTable(
                name: "Report_Reports",
                schema: "threddit");

            migrationBuilder.DropTable(
                name: "SiteOwner",
                schema: "threddit");

            migrationBuilder.DropTable(
                name: "User_BannedSiteUsers",
                schema: "threddit");

            migrationBuilder.DropTable(
                name: "User_BannedSubThreadUsers",
                schema: "threddit");

            migrationBuilder.DropIndex(
                name: "IX_User_FollowedUsers_FollowerId_FollowedId",
                schema: "threddit",
                table: "User_FollowedUsers");

            migrationBuilder.DropIndex(
                name: "IX_User_BlockedUsers_BlockerId_BlockedId",
                schema: "threddit",
                table: "User_BlockedUsers");

            migrationBuilder.DropIndex(
                name: "IX_Post_Posts_PostedAt",
                schema: "threddit",
                table: "Post_Posts");

            migrationBuilder.DropIndex(
                name: "IX_Post_Posts_SubThreadId_Slug",
                schema: "threddit",
                table: "Post_Posts");

            migrationBuilder.DropIndex(
                name: "IX_Post_Posts_SubThreadId_Title",
                schema: "threddit",
                table: "Post_Posts");

            migrationBuilder.DropIndex(
                name: "IX_Conversation_DirectMessages_ParentMessageId",
                schema: "threddit",
                table: "Conversation_DirectMessages");

            migrationBuilder.DropIndex(
                name: "IX_Comment_Comments_CommentedAt",
                schema: "threddit",
                table: "Comment_Comments");

            migrationBuilder.DropColumn(
                name: "Permissions",
                schema: "threddit",
                table: "User_Moderators");

            migrationBuilder.DropColumn(
                name: "RuleContent",
                schema: "threddit",
                table: "SubThread_SubThreadRules");

            migrationBuilder.DropColumn(
                name: "RuleTitle",
                schema: "threddit",
                table: "SubThread_SubThreadRules");

            migrationBuilder.DropColumn(
                name: "IsDownvote",
                schema: "threddit",
                table: "Post_PostVotes");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                schema: "threddit",
                table: "Post_Posts");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                schema: "threddit",
                table: "Post_Posts");

            migrationBuilder.DropColumn(
                name: "Slug",
                schema: "threddit",
                table: "Post_Posts");

            migrationBuilder.DropColumn(
                name: "IsReply",
                schema: "threddit",
                table: "Conversation_DirectMessages");

            migrationBuilder.DropColumn(
                name: "ParentMessageId",
                schema: "threddit",
                table: "Conversation_DirectMessages");

            migrationBuilder.RenameColumn(
                name: "PostedById",
                schema: "threddit",
                table: "Post_Posts",
                newName: "UserId");

            migrationBuilder.RenameIndex(
                name: "IX_Post_Posts_PostedById",
                schema: "threddit",
                table: "Post_Posts",
                newName: "IX_Post_Posts_UserId");

            migrationBuilder.RenameColumn(
                name: "CommentedById",
                schema: "threddit",
                table: "Comment_Comments",
                newName: "UserId");

            migrationBuilder.RenameIndex(
                name: "IX_Comment_Comments_CommentedById",
                schema: "threddit",
                table: "Comment_Comments",
                newName: "IX_Comment_Comments_UserId");

            migrationBuilder.AlterColumn<Guid>(
                name: "FollowerId",
                schema: "threddit",
                table: "User_FollowedUsers",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "FollowedId",
                schema: "threddit",
                table: "User_FollowedUsers",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "BlockerId",
                schema: "threddit",
                table: "User_BlockedUsers",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "BlockedId",
                schema: "threddit",
                table: "User_BlockedUsers",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Rule",
                schema: "threddit",
                table: "SubThread_SubThreadRules",
                type: "nvarchar(250)",
                maxLength: 250,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<Guid>(
                name: "SubThreadId",
                schema: "threddit",
                table: "Post_PinnedSubThreadPosts",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.CreateTable(
                name: "User_BannedUsers",
                schema: "threddit",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SubThreadId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BannedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ExpiresAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Reason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_User_BannedUsers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_User_BannedUsers_SubThread_SubThreads_SubThreadId",
                        column: x => x.SubThreadId,
                        principalSchema: "threddit",
                        principalTable: "SubThread_SubThreads",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_User_BannedUsers_User_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "threddit",
                        principalTable: "User_Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_User_FollowedUsers_FollowerId_FollowedId",
                schema: "threddit",
                table: "User_FollowedUsers",
                columns: new[] { "FollowerId", "FollowedId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_User_BlockedUsers_BlockerId_BlockedId",
                schema: "threddit",
                table: "User_BlockedUsers",
                columns: new[] { "BlockerId", "BlockedId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Post_Posts_SubThreadId",
                schema: "threddit",
                table: "Post_Posts",
                column: "SubThreadId");

            migrationBuilder.CreateIndex(
                name: "IX_User_BannedUsers_SubThreadId",
                schema: "threddit",
                table: "User_BannedUsers",
                column: "SubThreadId");

            migrationBuilder.CreateIndex(
                name: "IX_User_BannedUsers_UserId_SubThreadId",
                schema: "threddit",
                table: "User_BannedUsers",
                columns: new[] { "UserId", "SubThreadId" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Comment_Comments_User_Users_UserId",
                schema: "threddit",
                table: "Comment_Comments",
                column: "UserId",
                principalSchema: "threddit",
                principalTable: "User_Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Comment_CommentVotes_User_Users_UserId",
                schema: "threddit",
                table: "Comment_CommentVotes",
                column: "UserId",
                principalSchema: "threddit",
                principalTable: "User_Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Conversation_Conversations_User_Users_InitiatorId",
                schema: "threddit",
                table: "Conversation_Conversations",
                column: "InitiatorId",
                principalSchema: "threddit",
                principalTable: "User_Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Conversation_Conversations_User_Users_RecipientId",
                schema: "threddit",
                table: "Conversation_Conversations",
                column: "RecipientId",
                principalSchema: "threddit",
                principalTable: "User_Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Conversation_DirectMessages_User_Users_SenderId",
                schema: "threddit",
                table: "Conversation_DirectMessages",
                column: "SenderId",
                principalSchema: "threddit",
                principalTable: "User_Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Post_PinnedSubThreadPosts_SubThread_SubThreads_SubThreadId",
                schema: "threddit",
                table: "Post_PinnedSubThreadPosts",
                column: "SubThreadId",
                principalSchema: "threddit",
                principalTable: "SubThread_SubThreads",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Post_PinnedSubThreadPosts_User_Users_PinnedById",
                schema: "threddit",
                table: "Post_PinnedSubThreadPosts",
                column: "PinnedById",
                principalSchema: "threddit",
                principalTable: "User_Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Post_Posts_User_Users_UserId",
                schema: "threddit",
                table: "Post_Posts",
                column: "UserId",
                principalSchema: "threddit",
                principalTable: "User_Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Post_PostVotes_User_Users_UserId",
                schema: "threddit",
                table: "Post_PostVotes",
                column: "UserId",
                principalSchema: "threddit",
                principalTable: "User_Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_SubThread_SubThreadRules_User_Users_CreatedById",
                schema: "threddit",
                table: "SubThread_SubThreadRules",
                column: "CreatedById",
                principalSchema: "threddit",
                principalTable: "User_Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_SubThread_SubThreadRules_User_Users_LastUpdatedById",
                schema: "threddit",
                table: "SubThread_SubThreadRules",
                column: "LastUpdatedById",
                principalSchema: "threddit",
                principalTable: "User_Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_SubThread_SubThreads_User_Users_CreatedById",
                schema: "threddit",
                table: "SubThread_SubThreads",
                column: "CreatedById",
                principalSchema: "threddit",
                principalTable: "User_Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }
    }
}
