using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Threddit.Domain.Entities;

namespace Threddit.Infrastructure.Persistence;

public sealed class ThredditDbContext(DbContextOptions<ThredditDbContext> options)
    : IdentityDbContext<User, IdentityRole<Guid>, Guid>(options)
{
    public DbSet<SubThreadModerator> SubThreadModerators => Set<SubThreadModerator>();
    public DbSet<SubThreadOwner> SubThreadOwners => Set<SubThreadOwner>();
    public DbSet<SiteAdmin> SiteAdmins => Set<SiteAdmin>();
    public DbSet<SiteOwner> SiteOwner => Set<SiteOwner>();
    public DbSet<BannedSubThreadUser> BannedSubThreadUsers => Set<BannedSubThreadUser>();
    public DbSet<BannedSiteUser> BannedSiteUsers => Set<BannedSiteUser>();
    public DbSet<BlockedUser> BlockedUsers => Set<BlockedUser>();
    public DbSet<FollowedUser> FollowedUsers => Set<FollowedUser>();
    public DbSet<Post> Posts => Set<Post>();
    public DbSet<PinnedSubThreadPost> PinnedSubThreadPosts => Set<PinnedSubThreadPost>();
    public DbSet<SavedPost> SavedPosts => Set<SavedPost>();
    public DbSet<PostVote> PostVotes => Set<PostVote>();
    public DbSet<PostView> PostViews => Set<PostView>();
    public DbSet<Comment> Comments => Set<Comment>();
    public DbSet<SavedComment> SavedComments => Set<SavedComment>();
    public DbSet<CommentVote> CommentVotes => Set<CommentVote>();
    public DbSet<SubThread> SubThreads => Set<SubThread>();
    public DbSet<SubThreadSubscription> SubThreadSubscriptions => Set<SubThreadSubscription>();
    public DbSet<SubThreadRule> SubThreadRules => Set<SubThreadRule>();
    public DbSet<DirectMessage> DirectMessages => Set<DirectMessage>();
    public DbSet<DirectMessageRead> DirectMessageReads => Set<DirectMessageRead>();
    public DbSet<Conversation> Conversations => Set<Conversation>();
    public DbSet<GroupConversation> GroupConversations => Set<GroupConversation>();
    public DbSet<GroupConversationMember> GroupConversationMembers => Set<GroupConversationMember>();
    public DbSet<Report> Reports => Set<Report>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfigurationsFromAssembly(typeof(ThredditDbContext).Assembly);
        builder.HasDefaultSchema("threddit");
    }
}