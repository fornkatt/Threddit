using Microsoft.AspNetCore.Identity;
using Threddit.Domain.Common;

namespace Threddit.Domain.Entities;

public sealed class User : IdentityUser<Guid>
{
    public static class Limits
    {
        public const int MaxUsernameLength = 40;
        public const int MaxEmailLength = 150;
        public const int MaxProfilePictureUrlLength = 2048;
        public const int MaxDescriptionLength = 500;
    }

    private User()
    {
    }

    public string? ProfilePicture { get; private set; }
    public string? Description { get; private set; }
    public int PostScore { get; private set; }
    public int CommentScore { get; private set; }
    public int TotalScore { get; private set; }

    public BannedSiteUser? BannedSiteUser { get; private set; }
    public SiteAdmin? SiteAdmin { get; private set; }
    public SiteOwner? SiteOwner { get; private set; }
    public DateTime CreationDate { get; init; } = DateTime.UtcNow;

    public IReadOnlyCollection<SubThreadModerator> SubThreadModeratorRoles => _subThreadModeratorRoles.AsReadOnly();
    public IReadOnlyCollection<SubThreadOwner> SubThreadOwnerRoles => _subThreadOwnerRoles.AsReadOnly();

    public IReadOnlyCollection<SubThread> CreatedSubThreads => _createdSubThreads.AsReadOnly();
    public IReadOnlyCollection<SubThreadSubscription> SubThreadSubscriptions => _subThreadSubscriptions.AsReadOnly();

    public IReadOnlyCollection<BlockedUser> BlockedUsers => _blockedUsers.AsReadOnly();
    public IReadOnlyCollection<BlockedUser> BlockedByUsers => _blockedByUsers.AsReadOnly();

    public IReadOnlyCollection<FollowedUser> FollowedUsers => _followedUsers.AsReadOnly();
    public IReadOnlyCollection<FollowedUser> FollowedByUsers => _followedByUsers.AsReadOnly();

    public IReadOnlyCollection<Post> Posts => _posts.AsReadOnly();
    public IReadOnlyCollection<SavedPost> SavedPosts => _savedPosts.AsReadOnly();
    public IReadOnlyCollection<PostVote> PostVotes => _postVotes.AsReadOnly();
    public IReadOnlyCollection<PostView> ViewedPostHistory => _viewedPostHistory.AsReadOnly();

    public IReadOnlyCollection<Comment> Comments => _comments.AsReadOnly();
    public IReadOnlyCollection<SavedComment> SavedComments => _savedComments.AsReadOnly();
    public IReadOnlyCollection<CommentVote> CommentVotes => _commentVotes.AsReadOnly();

    public IReadOnlyCollection<Conversation> InitiatedConversations => _initiatedConversations.AsReadOnly();
    public IReadOnlyCollection<Conversation> ReceivedConversations => _receivedConversations.AsReadOnly();

    public IReadOnlyCollection<GroupConversationMember> GroupConversationMemberships =>
        _groupConversationMemberships.AsReadOnly();

    public IReadOnlyCollection<BannedSubThreadUser> IssuedSubThreadBans => _issuedSubThreadBans.AsReadOnly();
    public IReadOnlyCollection<BannedSiteUser> IssuedSiteBans => _issuedSiteBans.AsReadOnly();
    public IReadOnlyCollection<BannedSubThreadUser> ReceivedSubThreadBans => _receivedSubThreadBans.AsReadOnly();

    public IReadOnlyCollection<Report> Reports => _reports.AsReadOnly();

    private readonly List<SubThreadModerator> _subThreadModeratorRoles = [];
    private readonly List<SubThreadOwner> _subThreadOwnerRoles = [];

    private readonly List<SubThread> _createdSubThreads = [];
    private readonly List<SubThreadSubscription> _subThreadSubscriptions = [];

    private readonly List<BlockedUser> _blockedUsers = [];
    private readonly List<BlockedUser> _blockedByUsers = [];

    private readonly List<FollowedUser> _followedUsers = [];
    private readonly List<FollowedUser> _followedByUsers = [];

    private readonly List<Post> _posts = [];
    private readonly List<SavedPost> _savedPosts = [];
    private readonly List<PostVote> _postVotes = [];
    private readonly List<PostView> _viewedPostHistory = [];

    private readonly List<Comment> _comments = [];
    private readonly List<SavedComment> _savedComments = [];
    private readonly List<CommentVote> _commentVotes = [];

    private readonly List<Conversation> _initiatedConversations = [];
    private readonly List<Conversation> _receivedConversations = [];

    private readonly List<GroupConversationMember> _groupConversationMemberships = [];

    private readonly List<BannedSubThreadUser> _issuedSubThreadBans = [];
    private readonly List<BannedSiteUser> _issuedSiteBans = [];
    private readonly List<BannedSubThreadUser> _receivedSubThreadBans = [];

    private readonly List<Report> _reports = [];

    /// <summary>Creates a new user.</summary>
    /// <remarks>
    /// Actual user creation is handled by Identity through the <see cref="UserManager{TUser}"/> class.
    /// <br/><br/>
    /// Possible error types:
    /// <list type="bullet">
    ///     <item><see cref="ErrorType.InvalidUsername"/></item>
    ///     <item><see cref="ErrorType.InvalidEmail"/></item>
    /// </list>
    /// </remarks>
    public static Result<User> Create(string username, string email)
    {
        if (string.IsNullOrWhiteSpace(username))
            return Result<User>.Error("Username cannot be empty.", ErrorType.InvalidUsername);

        if (string.IsNullOrWhiteSpace(email))
            return Result<User>.Error("Email cannot be empty.", ErrorType.InvalidEmail);
        
        if (username.Length > Limits.MaxUsernameLength)
            return Result<User>.Error($"Username cannot exceed {Limits.MaxUsernameLength} characters.",
                ErrorType.UsernameTooLong);
        
        if (email.Length > Limits.MaxEmailLength)
            return Result<User>.Error($"Email cannot exceed {Limits.MaxEmailLength} characters.",
                ErrorType.EmailTooLong);

        return Result<User>.Success(new User
        {
            UserName = username,
            Email = email,
            EmailConfirmed = true
        });
    }

    public void ApplyPostScoreDelta(int delta)
    {
        PostScore += delta;
        TotalScore += delta;
    }

    public void ApplyCommentScoreDelta(int delta)
    {
        CommentScore += delta;
        TotalScore += delta;
    }

    /// <summary>Edits an existing user.</summary>
    /// <remarks>
    /// Possible error types:
    /// <list type="bullet">
    ///     <item><see cref="ErrorType.ContentTooLong"/></item>
    ///     <item><see cref="ErrorType.ImageUrlTooLong"/></item>
    /// </list>
    /// </remarks>
    public Result Edit(string? profilePicture = null, string? description = null)
    {
        if (profilePicture is { Length: > Limits.MaxProfilePictureUrlLength })
            return Result.Error($"Image URL cannot exceed {Limits.MaxProfilePictureUrlLength} characters.",
                ErrorType.ImageUrlTooLong);

        if (description is { Length: > Limits.MaxDescriptionLength })
            return Result.Error($"Description cannot exceed {Limits.MaxDescriptionLength} characters.",
                ErrorType.ContentTooLong);

        ProfilePicture = profilePicture;
        Description = description;

        return Result.Success();
    }
}

public sealed class SubThreadModerator
{
    private SubThreadModerator()
    {
    }

    public Guid Id { get; init; } = Guid.CreateVersion7();
    public User User { get; init; } = null!;
    public Guid UserId { get; init; }
    public SubThread SubThread { get; init; } = null!;
    public Guid SubThreadId { get; init; }
    public DateTime AssignedAt { get; init; } = DateTime.UtcNow;

    public ModeratorPermissions Permissions { get; private set; } = ModeratorPermissions.None;

    [Flags]
    public enum ModeratorPermissions
    {
        None = 0,
        Posts = 1 << 0,
        Bans = 1 << 1,
        Settings = 1 << 2,
        All = ~0
    }

    public static SubThreadModerator Assign(User user, SubThread subThread)
    {
        return new SubThreadModerator
        {
            User = user,
            UserId = user.Id,
            SubThread = subThread,
            SubThreadId = subThread.Id,
        };
    }
}

public sealed class SubThreadOwner
{
    private SubThreadOwner()
    {
    }

    public Guid Id { get; init; } = Guid.CreateVersion7();
    public User? User { get; private set; }
    public Guid? UserId { get; private set; }
    public SubThread SubThread { get; init; } = null!;
    public Guid SubThreadId { get; init; }
    public DateTime AssignedAt { get; init; } = DateTime.UtcNow;

    internal static SubThreadOwner Assign(User user, SubThread subThread)
    {
        return new SubThreadOwner
        {
            User = user,
            UserId = user.Id,
            SubThread = subThread,
            SubThreadId = subThread.Id,
        };
    }

    internal void Reassign(User user)
    {
        User = user;
        UserId = user.Id;
    }
}

public sealed class SiteAdmin
{
    private SiteAdmin()
    {
    }

    public Guid Id { get; init; } = Guid.CreateVersion7();
    public User User { get; private set; } = null!;
    public Guid UserId { get; private set; }
    public DateTime AssignedAt { get; init; } = DateTime.UtcNow;

    public static SiteAdmin Assign(User user)
    {
        return new SiteAdmin
        {
            User = user,
            UserId = user.Id,
        };
    }
}

public sealed class SiteOwner
{
    private SiteOwner()
    {
    }

    public int SingletonKey { get; private set; } = 1;
    public Guid Id { get; init; } = Guid.CreateVersion7();
    public User User { get; private set; } = null!;
    public Guid UserId { get; private set; }
    public DateTime AssignedAt { get; init; } = DateTime.UtcNow;

    public static SiteOwner Assign(User user)
    {
        return new SiteOwner
        {
            User = user,
            UserId = user.Id,
        };
    }

    /// <summary>Reassigns the site owner.</summary>
    /// <remarks>
    /// Possible error types:
    /// <list type="bullet">
    ///     <item><see cref="ErrorType.AlreadyAssigned"/></item>
    /// </list>
    /// </remarks>
    public Result ReAssign(User user)
    {
        if (User is { } oldOwner && user.Id == oldOwner.Id)
            return Result.Error("New owner cannot be the same as old owner.", ErrorType.AlreadyAssigned);

        User = user;
        UserId = user.Id;

        return Result.Success();
    }
}

public sealed class BannedSiteUser
{
    public static class Limits
    {
        public const int MaxReasonLength = 500;
    }
    
    private BannedSiteUser()
    {
    }

    public Guid Id { get; init; } = Guid.CreateVersion7();
    public string Reason { get; private set; } = null!;
    public User User { get; init; } = null!;
    public Guid UserId { get; init; }
    public DateTime BannedAt { get; init; } = DateTime.UtcNow;
    public DateTime ExpiresAt { get; private set; }

    public User? BannedBy { get; private set; }
    public Guid? BannedById { get; private set; }
    public User? LastEditedBy { get; private set; }
    public Guid? LastEditedById { get; private set; }
    public DateTime? EditedAt { get; private set; }

    public bool IsExpired => DateTime.UtcNow > ExpiresAt;

    /// <summary>Bans a user from the site.</summary>
    /// <remarks>
    /// Possible error types:
    /// <list type="bullet">
    ///     <item><see cref="ErrorType.BanReasonEmpty"/></item>
    ///     <item><see cref="ErrorType.InvalidBanDate"/></item>
    ///     <item><see cref="ErrorType.BanReasonTooLong"/></item>
    /// </list>
    /// </remarks>
    public static Result<BannedSiteUser> Create(User user, User bannedBy, string reason,
        DateTime expiresAt)
    {
        if (string.IsNullOrWhiteSpace(reason))
            return Result<BannedSiteUser>.Error("Reason cannot be empty.", ErrorType.BanReasonEmpty);

        if (expiresAt < DateTime.UtcNow)
            return Result<BannedSiteUser>.Error("Expiry date can't be in the past.", ErrorType.InvalidBanDate);
        
        if (reason.Length > Limits.MaxReasonLength)
            return Result<BannedSiteUser>.Error($"Reason cannot exceed {Limits.MaxReasonLength} characters.",
                ErrorType.BanReasonTooLong);

        return Result<BannedSiteUser>.Success(new BannedSiteUser
        {
            User = user,
            UserId = user.Id,
            Reason = reason,
            BannedBy = bannedBy,
            BannedById = bannedBy.Id,
            ExpiresAt = expiresAt,
        });
    }

    /// <summary>Edits an existing site ban.</summary>
    /// <remarks>
    /// Possible error types:
    /// <list type="bullet">
    ///     <item><see cref="ErrorType.BanReasonEmpty"/></item>
    ///     <item><see cref="ErrorType.BanReasonTooLong"/></item>
    /// </list>
    /// </remarks>
    public Result Edit(string newReason, User lastUpdatedBy, DateTime? newExpiryDate = null)
    {
        if (string.IsNullOrWhiteSpace(newReason))
            return Result.Error("Reason cannot be empty.", ErrorType.BanReasonEmpty);

        if (newExpiryDate.HasValue)
            ExpiresAt = newExpiryDate.Value;
        
        if (newReason.Length > Limits.MaxReasonLength)
            return Result.Error($"Reason cannot exceed {Limits.MaxReasonLength} characters.", ErrorType.BanReasonTooLong);

        LastEditedBy = lastUpdatedBy;
        LastEditedById = lastUpdatedBy.Id;
        Reason = newReason;
        EditedAt = DateTime.UtcNow;

        return Result.Success();
    }
}

public sealed class BannedSubThreadUser
{
    public static class Limits
    {
        public const int MaxReasonLength = 500;
    }

    private BannedSubThreadUser()
    {
    }

    public Guid Id { get; init; } = Guid.CreateVersion7();
    public string Reason { get; private set; } = null!;
    public User User { get; init; } = null!;
    public Guid UserId { get; init; }
    public SubThread SubThread { get; init; } = null!;
    public Guid SubThreadId { get; init; }
    public DateTime BannedAt { get; init; } = DateTime.UtcNow;
    public DateTime ExpiresAt { get; private set; }
    public DateTime? EditedAt { get; private set; }
    public User? BannedBy { get; init; }
    public Guid? BannedById { get; init; }
    public User? LastEditedBy { get; private set; }
    public Guid? LastEditedById { get; private set; }

    public bool IsExpired => DateTime.UtcNow > ExpiresAt;

    /// <summary>Bans a user from a specific SubThread.</summary>
    /// <remarks>
    /// Possible error types:
    /// <list type="bullet">
    ///     <item><see cref="ErrorType.BanReasonEmpty"/></item>
    ///     <item><see cref="ErrorType.InvalidBanDate"/></item>
    ///     <item><see cref="ErrorType.BanReasonTooLong"/></item>
    /// </list>
    /// </remarks>
    public static Result<BannedSubThreadUser> Create(User user, User bannedBy, SubThread subThread, string reason,
        DateTime expiresAt)
    {
        if (string.IsNullOrWhiteSpace(reason))
            return Result<BannedSubThreadUser>.Error("Reason cannot be empty.", ErrorType.BanReasonEmpty);

        if (expiresAt < DateTime.UtcNow)
            return Result<BannedSubThreadUser>.Error("Expiry date can't be in the past.", ErrorType.InvalidBanDate);

        if (reason.Length > Limits.MaxReasonLength)
            return Result<BannedSubThreadUser>.Error($"Reason cannot exceed {Limits.MaxReasonLength} characters.",
                ErrorType.BanReasonTooLong);

        return Result<BannedSubThreadUser>.Success(new BannedSubThreadUser
        {
            User = user,
            UserId = user.Id,
            SubThread = subThread,
            SubThreadId = subThread.Id,
            Reason = reason,
            BannedBy = bannedBy,
            BannedById = bannedBy.Id,
            ExpiresAt = expiresAt,
        });
    }

    /// <summary>Edits an existing SubThread ban.</summary>
    /// <remarks>
    /// Possible error types:
    /// <list type="bullet">
    ///     <item><see cref="ErrorType.BanReasonEmpty"/></item>
    ///     <item><see cref="ErrorType.BanReasonTooLong"/></item>
    /// </list>
    /// </remarks>
    public Result Edit(string newReason, User lastUpdatedBy, DateTime? newExpiryDate = null)
    {
        if (string.IsNullOrWhiteSpace(newReason))
            return Result.Error("Reason cannot be empty.", ErrorType.BanReasonEmpty);

        if (newExpiryDate.HasValue)
            ExpiresAt = newExpiryDate.Value;

        if (newReason.Length > Limits.MaxReasonLength)
            return Result.Error($"Reason cannot exceed {Limits.MaxReasonLength} characters.",
                ErrorType.BanReasonTooLong);

        LastEditedBy = lastUpdatedBy;
        LastEditedById = lastUpdatedBy.Id;
        Reason = newReason;
        EditedAt = DateTime.UtcNow;

        return Result.Success();
    }
}

public sealed class BlockedUser
{
    private BlockedUser()
    {
    }

    public Guid Id { get; init; } = Guid.CreateVersion7();
    public User? Blocker { get; init; }
    public Guid? BlockerId { get; init; }
    public User? Blocked { get; init; }
    public Guid? BlockedId { get; init; }
    public DateTime BlockedAt { get; init; } = DateTime.UtcNow;

    public static BlockedUser Create(User blocker, User blocked)
    {
        return new BlockedUser
        {
            Blocker = blocker,
            BlockerId = blocker.Id,
            Blocked = blocked,
            BlockedId = blocked.Id,
        };
    }
}

public sealed class FollowedUser
{
    private FollowedUser()
    {
    }

    public Guid Id { get; init; } = Guid.CreateVersion7();
    public User? Follower { get; init; }
    public Guid? FollowerId { get; init; }
    public User? Followed { get; init; }
    public Guid? FollowedId { get; init; }
    public DateTime FollowedAt { get; init; } = DateTime.UtcNow;

    public static FollowedUser Create(User follower, User followed)
    {
        return new FollowedUser
        {
            Follower = follower,
            FollowerId = follower.Id,
            Followed = followed,
            FollowedId = followed.Id,
        };
    }
}