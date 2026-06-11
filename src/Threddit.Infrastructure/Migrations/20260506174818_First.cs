using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Threddit.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class First : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "threddit");

            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                schema: "threddit",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "User_Users",
                schema: "threddit",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProfilePicture = table.Column<string>(type: "nvarchar(2048)", maxLength: 2048, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SecurityStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "bit", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "bit", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_User_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                schema: "threddit",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalSchema: "threddit",
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                schema: "threddit",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_User_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "threddit",
                        principalTable: "User_Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                schema: "threddit",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderKey = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_User_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "threddit",
                        principalTable: "User_Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                schema: "threddit",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RoleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalSchema: "threddit",
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_User_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "threddit",
                        principalTable: "User_Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                schema: "threddit",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_User_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "threddit",
                        principalTable: "User_Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Conversation_Conversations",
                schema: "threddit",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    InitiatorId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    RecipientId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    InitiatorHidden = table.Column<bool>(type: "bit", nullable: false),
                    RecipientHidden = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Conversation_Conversations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Conversation_Conversations_User_Users_InitiatorId",
                        column: x => x.InitiatorId,
                        principalSchema: "threddit",
                        principalTable: "User_Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Conversation_Conversations_User_Users_RecipientId",
                        column: x => x.RecipientId,
                        principalSchema: "threddit",
                        principalTable: "User_Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "SubThread_SubThreads",
                schema: "threddit",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedById = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    BannerImageUrl = table.Column<string>(type: "nvarchar(2048)", maxLength: 2048, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubThread_SubThreads", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SubThread_SubThreads_User_Users_CreatedById",
                        column: x => x.CreatedById,
                        principalSchema: "threddit",
                        principalTable: "User_Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "User_BlockedUsers",
                schema: "threddit",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BlockerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BlockedId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BlockedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_User_BlockedUsers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_User_BlockedUsers_User_Users_BlockedId",
                        column: x => x.BlockedId,
                        principalSchema: "threddit",
                        principalTable: "User_Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_User_BlockedUsers_User_Users_BlockerId",
                        column: x => x.BlockerId,
                        principalSchema: "threddit",
                        principalTable: "User_Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "User_FollowedUsers",
                schema: "threddit",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FollowerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FollowedId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FollowedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_User_FollowedUsers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_User_FollowedUsers_User_Users_FollowedId",
                        column: x => x.FollowedId,
                        principalSchema: "threddit",
                        principalTable: "User_Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_User_FollowedUsers_User_Users_FollowerId",
                        column: x => x.FollowerId,
                        principalSchema: "threddit",
                        principalTable: "User_Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "User_SiteAdmins",
                schema: "threddit",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AssignedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_User_SiteAdmins", x => x.Id);
                    table.ForeignKey(
                        name: "FK_User_SiteAdmins_User_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "threddit",
                        principalTable: "User_Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Conversation_DirectMessages",
                schema: "threddit",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SenderId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ConversationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Content = table.Column<string>(type: "nvarchar(2500)", maxLength: 2500, nullable: true),
                    SentAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EditedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    IsRead = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Conversation_DirectMessages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Conversation_DirectMessages_Conversation_Conversations_ConversationId",
                        column: x => x.ConversationId,
                        principalSchema: "threddit",
                        principalTable: "Conversation_Conversations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Conversation_DirectMessages_User_Users_SenderId",
                        column: x => x.SenderId,
                        principalSchema: "threddit",
                        principalTable: "User_Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "Post_Posts",
                schema: "threddit",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    SubThreadId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Content = table.Column<string>(type: "nvarchar(3000)", maxLength: 3000, nullable: false),
                    ImageUrl = table.Column<string>(type: "nvarchar(2048)", maxLength: 2048, nullable: true),
                    PostedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Post_Posts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Post_Posts_SubThread_SubThreads_SubThreadId",
                        column: x => x.SubThreadId,
                        principalSchema: "threddit",
                        principalTable: "SubThread_SubThreads",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Post_Posts_User_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "threddit",
                        principalTable: "User_Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "SubThread_SubThreadRules",
                schema: "threddit",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SubThreadId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Rule = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    CreatedById = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    LastUpdatedById = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubThread_SubThreadRules", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SubThread_SubThreadRules_SubThread_SubThreads_SubThreadId",
                        column: x => x.SubThreadId,
                        principalSchema: "threddit",
                        principalTable: "SubThread_SubThreads",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SubThread_SubThreadRules_User_Users_CreatedById",
                        column: x => x.CreatedById,
                        principalSchema: "threddit",
                        principalTable: "User_Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_SubThread_SubThreadRules_User_Users_LastUpdatedById",
                        column: x => x.LastUpdatedById,
                        principalSchema: "threddit",
                        principalTable: "User_Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "SubThread_SubThreadSubscriptions",
                schema: "threddit",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SubThreadId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SubscribedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubThread_SubThreadSubscriptions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SubThread_SubThreadSubscriptions_SubThread_SubThreads_SubThreadId",
                        column: x => x.SubThreadId,
                        principalSchema: "threddit",
                        principalTable: "SubThread_SubThreads",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SubThread_SubThreadSubscriptions_User_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "threddit",
                        principalTable: "User_Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "User_BannedUsers",
                schema: "threddit",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Reason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SubThreadId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BannedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ExpiresAt = table.Column<DateTime>(type: "datetime2", nullable: false)
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

            migrationBuilder.CreateTable(
                name: "User_Moderators",
                schema: "threddit",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SubThreadId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AssignedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_User_Moderators", x => x.Id);
                    table.ForeignKey(
                        name: "FK_User_Moderators_SubThread_SubThreads_SubThreadId",
                        column: x => x.SubThreadId,
                        principalSchema: "threddit",
                        principalTable: "SubThread_SubThreads",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_User_Moderators_User_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "threddit",
                        principalTable: "User_Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "User_SubThreadOwners",
                schema: "threddit",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    SubThreadId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AssignedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_User_SubThreadOwners", x => x.Id);
                    table.ForeignKey(
                        name: "FK_User_SubThreadOwners_SubThread_SubThreads_SubThreadId",
                        column: x => x.SubThreadId,
                        principalSchema: "threddit",
                        principalTable: "SubThread_SubThreads",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_User_SubThreadOwners_User_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "threddit",
                        principalTable: "User_Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Comment_Comments",
                schema: "threddit",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    PostId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Content = table.Column<string>(type: "nvarchar(2500)", maxLength: 2500, nullable: true),
                    CommentedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ParentCommentId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Comment_Comments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Comment_Comments_Comment_Comments_ParentCommentId",
                        column: x => x.ParentCommentId,
                        principalSchema: "threddit",
                        principalTable: "Comment_Comments",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Comment_Comments_Post_Posts_PostId",
                        column: x => x.PostId,
                        principalSchema: "threddit",
                        principalTable: "Post_Posts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Comment_Comments_User_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "threddit",
                        principalTable: "User_Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "Post_PinnedSubThreadPosts",
                schema: "threddit",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PostId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PinnedById = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    PinnedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    SubThreadId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Post_PinnedSubThreadPosts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Post_PinnedSubThreadPosts_Post_Posts_PostId",
                        column: x => x.PostId,
                        principalSchema: "threddit",
                        principalTable: "Post_Posts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Post_PinnedSubThreadPosts_SubThread_SubThreads_SubThreadId",
                        column: x => x.SubThreadId,
                        principalSchema: "threddit",
                        principalTable: "SubThread_SubThreads",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Post_PinnedSubThreadPosts_User_Users_PinnedById",
                        column: x => x.PinnedById,
                        principalSchema: "threddit",
                        principalTable: "User_Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "Post_PostViews",
                schema: "threddit",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PostId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ViewedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Post_PostViews", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Post_PostViews_Post_Posts_PostId",
                        column: x => x.PostId,
                        principalSchema: "threddit",
                        principalTable: "Post_Posts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Post_PostViews_User_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "threddit",
                        principalTable: "User_Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Post_PostVotes",
                schema: "threddit",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PostId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsUpvote = table.Column<bool>(type: "bit", nullable: false),
                    VotedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Post_PostVotes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Post_PostVotes_Post_Posts_PostId",
                        column: x => x.PostId,
                        principalSchema: "threddit",
                        principalTable: "Post_Posts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Post_PostVotes_User_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "threddit",
                        principalTable: "User_Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "Post_SavedPosts",
                schema: "threddit",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PostId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SavedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Post_SavedPosts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Post_SavedPosts_Post_Posts_PostId",
                        column: x => x.PostId,
                        principalSchema: "threddit",
                        principalTable: "Post_Posts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Post_SavedPosts_User_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "threddit",
                        principalTable: "User_Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Comment_CommentVotes",
                schema: "threddit",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CommentId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsUpvote = table.Column<bool>(type: "bit", nullable: false),
                    IsDownvote = table.Column<bool>(type: "bit", nullable: false),
                    VotedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Comment_CommentVotes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Comment_CommentVotes_Comment_Comments_CommentId",
                        column: x => x.CommentId,
                        principalSchema: "threddit",
                        principalTable: "Comment_Comments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Comment_CommentVotes_User_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "threddit",
                        principalTable: "User_Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "Comment_SavedComments",
                schema: "threddit",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CommentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SavedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Comment_SavedComments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Comment_SavedComments_Comment_Comments_CommentId",
                        column: x => x.CommentId,
                        principalSchema: "threddit",
                        principalTable: "Comment_Comments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Comment_SavedComments_User_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "threddit",
                        principalTable: "User_Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                schema: "threddit",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                schema: "threddit",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true,
                filter: "[NormalizedName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                schema: "threddit",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                schema: "threddit",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                schema: "threddit",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_Comment_Comments_ParentCommentId",
                schema: "threddit",
                table: "Comment_Comments",
                column: "ParentCommentId");

            migrationBuilder.CreateIndex(
                name: "IX_Comment_Comments_PostId",
                schema: "threddit",
                table: "Comment_Comments",
                column: "PostId");

            migrationBuilder.CreateIndex(
                name: "IX_Comment_Comments_UserId",
                schema: "threddit",
                table: "Comment_Comments",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Comment_CommentVotes_CommentId",
                schema: "threddit",
                table: "Comment_CommentVotes",
                column: "CommentId");

            migrationBuilder.CreateIndex(
                name: "IX_Comment_CommentVotes_UserId_CommentId",
                schema: "threddit",
                table: "Comment_CommentVotes",
                columns: new[] { "UserId", "CommentId" },
                unique: true,
                filter: "[UserId] IS NOT NULL AND [CommentId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Comment_SavedComments_CommentId",
                schema: "threddit",
                table: "Comment_SavedComments",
                column: "CommentId");

            migrationBuilder.CreateIndex(
                name: "IX_Comment_SavedComments_UserId_CommentId",
                schema: "threddit",
                table: "Comment_SavedComments",
                columns: new[] { "UserId", "CommentId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Conversation_Conversations_InitiatorId_RecipientId",
                schema: "threddit",
                table: "Conversation_Conversations",
                columns: new[] { "InitiatorId", "RecipientId" },
                unique: true,
                filter: "[InitiatorId] IS NOT NULL AND [RecipientId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Conversation_Conversations_RecipientId",
                schema: "threddit",
                table: "Conversation_Conversations",
                column: "RecipientId");

            migrationBuilder.CreateIndex(
                name: "IX_Conversation_DirectMessages_ConversationId",
                schema: "threddit",
                table: "Conversation_DirectMessages",
                column: "ConversationId");

            migrationBuilder.CreateIndex(
                name: "IX_Conversation_DirectMessages_SenderId",
                schema: "threddit",
                table: "Conversation_DirectMessages",
                column: "SenderId");

            migrationBuilder.CreateIndex(
                name: "IX_Post_PinnedSubThreadPosts_PinnedById",
                schema: "threddit",
                table: "Post_PinnedSubThreadPosts",
                column: "PinnedById");

            migrationBuilder.CreateIndex(
                name: "IX_Post_PinnedSubThreadPosts_PostId",
                schema: "threddit",
                table: "Post_PinnedSubThreadPosts",
                column: "PostId");

            migrationBuilder.CreateIndex(
                name: "IX_Post_PinnedSubThreadPosts_SubThreadId",
                schema: "threddit",
                table: "Post_PinnedSubThreadPosts",
                column: "SubThreadId");

            migrationBuilder.CreateIndex(
                name: "IX_Post_Posts_SubThreadId",
                schema: "threddit",
                table: "Post_Posts",
                column: "SubThreadId");

            migrationBuilder.CreateIndex(
                name: "IX_Post_Posts_UserId",
                schema: "threddit",
                table: "Post_Posts",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Post_PostViews_PostId_UserId",
                schema: "threddit",
                table: "Post_PostViews",
                columns: new[] { "PostId", "UserId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Post_PostViews_UserId",
                schema: "threddit",
                table: "Post_PostViews",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Post_PostVotes_PostId_UserId",
                schema: "threddit",
                table: "Post_PostVotes",
                columns: new[] { "PostId", "UserId" },
                unique: true,
                filter: "[PostId] IS NOT NULL AND [UserId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Post_PostVotes_UserId",
                schema: "threddit",
                table: "Post_PostVotes",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Post_SavedPosts_PostId",
                schema: "threddit",
                table: "Post_SavedPosts",
                column: "PostId");

            migrationBuilder.CreateIndex(
                name: "IX_Post_SavedPosts_UserId_PostId",
                schema: "threddit",
                table: "Post_SavedPosts",
                columns: new[] { "UserId", "PostId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SubThread_SubThreadRules_CreatedById",
                schema: "threddit",
                table: "SubThread_SubThreadRules",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_SubThread_SubThreadRules_LastUpdatedById",
                schema: "threddit",
                table: "SubThread_SubThreadRules",
                column: "LastUpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_SubThread_SubThreadRules_SubThreadId",
                schema: "threddit",
                table: "SubThread_SubThreadRules",
                column: "SubThreadId");

            migrationBuilder.CreateIndex(
                name: "IX_SubThread_SubThreads_CreatedById",
                schema: "threddit",
                table: "SubThread_SubThreads",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_SubThread_SubThreads_Name",
                schema: "threddit",
                table: "SubThread_SubThreads",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SubThread_SubThreadSubscriptions_SubThreadId",
                schema: "threddit",
                table: "SubThread_SubThreadSubscriptions",
                column: "SubThreadId");

            migrationBuilder.CreateIndex(
                name: "IX_SubThread_SubThreadSubscriptions_UserId_SubThreadId",
                schema: "threddit",
                table: "SubThread_SubThreadSubscriptions",
                columns: new[] { "UserId", "SubThreadId" },
                unique: true);

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

            migrationBuilder.CreateIndex(
                name: "IX_User_BlockedUsers_BlockedId",
                schema: "threddit",
                table: "User_BlockedUsers",
                column: "BlockedId");

            migrationBuilder.CreateIndex(
                name: "IX_User_BlockedUsers_BlockerId_BlockedId",
                schema: "threddit",
                table: "User_BlockedUsers",
                columns: new[] { "BlockerId", "BlockedId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_User_FollowedUsers_FollowedId",
                schema: "threddit",
                table: "User_FollowedUsers",
                column: "FollowedId");

            migrationBuilder.CreateIndex(
                name: "IX_User_FollowedUsers_FollowerId_FollowedId",
                schema: "threddit",
                table: "User_FollowedUsers",
                columns: new[] { "FollowerId", "FollowedId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_User_Moderators_SubThreadId",
                schema: "threddit",
                table: "User_Moderators",
                column: "SubThreadId");

            migrationBuilder.CreateIndex(
                name: "IX_User_Moderators_UserId_SubThreadId",
                schema: "threddit",
                table: "User_Moderators",
                columns: new[] { "UserId", "SubThreadId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_User_SiteAdmins_UserId",
                schema: "threddit",
                table: "User_SiteAdmins",
                column: "UserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_User_SubThreadOwners_SubThreadId",
                schema: "threddit",
                table: "User_SubThreadOwners",
                column: "SubThreadId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_User_SubThreadOwners_UserId",
                schema: "threddit",
                table: "User_SubThreadOwners",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                schema: "threddit",
                table: "User_Users",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                schema: "threddit",
                table: "User_Users",
                column: "NormalizedUserName",
                unique: true,
                filter: "[NormalizedUserName] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AspNetRoleClaims",
                schema: "threddit");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims",
                schema: "threddit");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins",
                schema: "threddit");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles",
                schema: "threddit");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens",
                schema: "threddit");

            migrationBuilder.DropTable(
                name: "Comment_CommentVotes",
                schema: "threddit");

            migrationBuilder.DropTable(
                name: "Comment_SavedComments",
                schema: "threddit");

            migrationBuilder.DropTable(
                name: "Conversation_DirectMessages",
                schema: "threddit");

            migrationBuilder.DropTable(
                name: "Post_PinnedSubThreadPosts",
                schema: "threddit");

            migrationBuilder.DropTable(
                name: "Post_PostViews",
                schema: "threddit");

            migrationBuilder.DropTable(
                name: "Post_PostVotes",
                schema: "threddit");

            migrationBuilder.DropTable(
                name: "Post_SavedPosts",
                schema: "threddit");

            migrationBuilder.DropTable(
                name: "SubThread_SubThreadRules",
                schema: "threddit");

            migrationBuilder.DropTable(
                name: "SubThread_SubThreadSubscriptions",
                schema: "threddit");

            migrationBuilder.DropTable(
                name: "User_BannedUsers",
                schema: "threddit");

            migrationBuilder.DropTable(
                name: "User_BlockedUsers",
                schema: "threddit");

            migrationBuilder.DropTable(
                name: "User_FollowedUsers",
                schema: "threddit");

            migrationBuilder.DropTable(
                name: "User_Moderators",
                schema: "threddit");

            migrationBuilder.DropTable(
                name: "User_SiteAdmins",
                schema: "threddit");

            migrationBuilder.DropTable(
                name: "User_SubThreadOwners",
                schema: "threddit");

            migrationBuilder.DropTable(
                name: "AspNetRoles",
                schema: "threddit");

            migrationBuilder.DropTable(
                name: "Comment_Comments",
                schema: "threddit");

            migrationBuilder.DropTable(
                name: "Conversation_Conversations",
                schema: "threddit");

            migrationBuilder.DropTable(
                name: "Post_Posts",
                schema: "threddit");

            migrationBuilder.DropTable(
                name: "SubThread_SubThreads",
                schema: "threddit");

            migrationBuilder.DropTable(
                name: "User_Users",
                schema: "threddit");
        }
    }
}
