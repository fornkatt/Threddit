using Threddit.Domain.Common;

namespace Threddit.Domain.Entities;

public sealed class SubThread
{
    public static class Limits
    {
        public const int MaxNameLength = 50;
        public const int MaxDescriptionLength = 1000;
        public const int MaxBannerImageUrlLength = 2048;
    }

    private SubThread()
    {
    }

    public Guid Id { get; init; } = Guid.CreateVersion7();
    public User? CreatedBy { get; init; }
    public Guid? CreatedById { get; init; }
    public SubThreadOwner SubThreadOwner { get; private set; } = null!;
    public string Name { get; private set; } = null!;
    public string? Description { get; private set; }
    public string? BannerImageUrl { get; private set; }
    public int SubscriberCount { get; private set; }
    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; private set; }

    public IReadOnlyCollection<SubThreadSubscription> Subscribers => _subscribers.AsReadOnly();
    public IReadOnlyCollection<BannedSubThreadUser> BannedUsers => _bannedUsers.AsReadOnly();
    public IReadOnlyCollection<SubThreadRule> SubThreadRules => _subThreadRules.AsReadOnly();
    public IReadOnlyCollection<Post> Posts => _posts.AsReadOnly();
    public IReadOnlyCollection<PinnedSubThreadPost> PinnedPosts => _pinnedPosts.AsReadOnly();
    public IReadOnlyCollection<SubThreadModerator> Moderators => _moderators.AsReadOnly();
    public IReadOnlyCollection<Report> Reports => _reports.AsReadOnly();

    private readonly List<SubThreadSubscription> _subscribers = [];
    private readonly List<BannedSubThreadUser> _bannedUsers = [];
    private readonly List<SubThreadRule> _subThreadRules = [];
    private readonly List<Post> _posts = [];
    private readonly List<PinnedSubThreadPost> _pinnedPosts = [];
    private readonly List<SubThreadModerator> _moderators = [];
    private readonly List<Report> _reports = [];

    /// <summary>Creates a new SubThread instance.</summary>
    /// <remarks>
    /// Possible error types:
    /// <list type="bullet">
    ///     <item><see cref="ErrorType.NameEmpty"/></item>
    ///     <item><see cref="ErrorType.NameTooLong"/></item>
    ///     <item><see cref="ErrorType.ContentTooLong"/></item>
    ///     <item><see cref="ErrorType.ImageUrlTooLong"/></item>
    /// </list>
    /// </remarks>
    public static Result<SubThread> Create(string name, User createdBy,
        string? description = null, string? bannerImageUrl = null)
    {
        if (string.IsNullOrWhiteSpace(name))
            return Result<SubThread>.Error("Name cannot be empty.", ErrorType.NameEmpty);

        if (name.Length > Limits.MaxNameLength)
            return Result<SubThread>
                .Error($"Name cannot exceed {Limits.MaxNameLength} characters.", ErrorType.NameTooLong);

        if (description is { Length: > Limits.MaxDescriptionLength })
            return Result<SubThread>
                .Error($"Description cannot exceed {Limits.MaxDescriptionLength} characters.",
                    ErrorType.ContentTooLong);

        if (bannerImageUrl is { Length: > Limits.MaxBannerImageUrlLength })
            return Result<SubThread>
                .Error($"Banner image URL cannot exceed {Limits.MaxBannerImageUrlLength} characters.",
                    ErrorType.ImageUrlTooLong);

        var subThread = new SubThread
        {
            Name = name,
            CreatedBy = createdBy,
            CreatedById = createdBy.Id,
            Description = description,
            BannerImageUrl = bannerImageUrl,
        };

        subThread.SubThreadOwner = SubThreadOwner.Assign(createdBy, subThread);

        return Result<SubThread>.Success(subThread);
    }

    /// <summary>Edits an existing SubThread object.</summary>
    /// <remarks>
    /// Possible error types:
    /// <list type="bullet">
    ///     <item><see cref="ErrorType.ContentTooLong"/></item>
    ///     <item><see cref="ErrorType.ImageUrlTooLong"/></item>
    /// </list>
    /// </remarks>
    public Result Edit(string? description = null, string? bannerImageUrl = null)
    {
        if (description is { Length: > Limits.MaxDescriptionLength })
            return Result.Error($"Description cannot exceed {Limits.MaxDescriptionLength} characters.",
                ErrorType.ContentTooLong);

        if (bannerImageUrl is { Length: > Limits.MaxBannerImageUrlLength })
            return Result.Error($"Banner image URL cannot exceed {Limits.MaxBannerImageUrlLength} characters.",
                ErrorType.ImageUrlTooLong);

        Description = description;
        BannerImageUrl = bannerImageUrl;
        UpdatedAt = DateTime.UtcNow;
        
        return Result.Success();
    }

    public void ApplySubscriberCountDelta(int delta)
    {
        SubscriberCount += delta;
    }

    /// <summary>Reassigns the owner of a SubThread.</summary>
    /// <remarks>
    /// Possible error types:
    /// <list type="bullet">
    ///     <item><see cref="ErrorType.AlreadyAssigned"/></item>
    /// </list>
    /// </remarks>
    public Result ReassignOwner(User newOwner)
    {
        if (SubThreadOwner.UserId == newOwner.Id)
            return Result.Error("New owner cannot be the same as old owner.", ErrorType.AlreadyAssigned);

        SubThreadOwner.Reassign(newOwner);
        
        UpdatedAt = DateTime.UtcNow;
        return Result.Success();
    }
}

public sealed class SubThreadSubscription
{
    private SubThreadSubscription()
    {
    }

    public Guid Id { get; init; } = Guid.CreateVersion7();
    public User User { get; init; } = null!;
    public Guid UserId { get; init; }
    public SubThread SubThread { get; init; } = null!;
    public Guid SubThreadId { get; init; }
    public DateTime SubscribedAt { get; init; } = DateTime.UtcNow;

    public static SubThreadSubscription Create(User user, SubThread subThread)
    {
        return new SubThreadSubscription
        {
            User = user,
            UserId = user.Id,
            SubThread = subThread,
            SubThreadId = subThread.Id,
        };
    }
}

public sealed class SubThreadRule
{
    public static class Limits
    {
        public const int MaxTitleLength = 100;
        public const int MaxContentLength = 300;
    }

    private SubThreadRule()
    {
    }

    public Guid Id { get; init; } = Guid.CreateVersion7();
    public SubThread SubThread { get; init; } = null!;
    public Guid SubThreadId { get; init; }
    public string RuleTitle { get; private set; } = null!;
    public string RuleContent { get; private set; } = null!;
    public User? CreatedBy { get; private set; }
    public Guid? CreatedById { get; private set; }
    public User? LastUpdatedBy { get; private set; }
    public Guid? LastUpdatedById { get; private set; }
    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; private set; }

    /// <summary>Creates a new rule for a SubThread.</summary>
    /// <remarks>
    /// Possible error types:
    /// <list type="bullet">
    ///     <item><see cref="ErrorType.TitleEmpty"/></item>
    ///     <item><see cref="ErrorType.ContentEmpty"/></item>
    ///     <item><see cref="ErrorType.TitleTooLong"/></item>
    ///     <item><see cref="ErrorType.ContentTooLong"/></item>
    /// </list>
    /// </remarks>
    public static Result<SubThreadRule> Create(SubThread subThread, string ruleTitle, string ruleContent,
        User createdBy)
    {
        if (string.IsNullOrWhiteSpace(ruleTitle))
            return Result<SubThreadRule>.Error("Rule title cannot be empty.", ErrorType.TitleEmpty);

        if (string.IsNullOrWhiteSpace(ruleContent))
            return Result<SubThreadRule>.Error("Rule content cannot be empty.", ErrorType.ContentEmpty);

        if (ruleTitle.Length > Limits.MaxTitleLength)
            return Result<SubThreadRule>
                .Error($"Rule title cannot exceed {Limits.MaxTitleLength} characters.",
                    ErrorType.TitleTooLong);

        if (ruleContent.Length > Limits.MaxContentLength)
            return Result<SubThreadRule>
                .Error($"Rule content cannot exceed {Limits.MaxContentLength} characters.",
                    ErrorType.ContentTooLong);

        return Result<SubThreadRule>.Success(new SubThreadRule
        {
            SubThread = subThread,
            SubThreadId = subThread.Id,
            RuleTitle = ruleTitle,
            RuleContent = ruleContent,
            CreatedBy = createdBy,
            CreatedById = createdBy.Id,
            CreatedAt = DateTime.UtcNow,
        });
    }

    /// <summary>Edits an existing SubThread rule.</summary>
    /// <remarks>
    /// Possible error types:
    /// <list type="bullet">
    ///     <item><see cref="ErrorType.TitleEmpty"/></item>
    ///     <item><see cref="ErrorType.ContentEmpty"/></item>
    ///     <item><see cref="ErrorType.TitleTooLong"/></item>
    ///     <item><see cref="ErrorType.ContentTooLong"/></item>
    /// </list>
    /// </remarks>
    public Result Edit(string ruleTitle, string ruleContent, User lastUpdatedBy)
    {
        if (string.IsNullOrWhiteSpace(ruleTitle))
            return Result.Error("Rule title cannot be empty.", ErrorType.TitleEmpty);

        if (string.IsNullOrWhiteSpace(ruleContent))
            return Result.Error("Rule content cannot be empty.", ErrorType.ContentEmpty);
        
        if (ruleTitle.Length > Limits.MaxTitleLength)
            return Result.Error($"Rule title cannot exceed {Limits.MaxTitleLength} characters.", ErrorType.TitleTooLong);

        if (ruleContent.Length > Limits.MaxContentLength)
            return Result
                .Error($"Rule content cannot exceed {Limits.MaxContentLength} characters.", ErrorType.ContentTooLong);

        RuleTitle = ruleTitle;
        RuleContent = ruleContent;
        LastUpdatedBy = lastUpdatedBy;
        LastUpdatedById = lastUpdatedBy.Id;
        UpdatedAt = DateTime.UtcNow;

        return Result.Success();
    }
}