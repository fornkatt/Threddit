using Threddit.Domain.Common;
using Threddit.Domain.Extensions;

namespace Threddit.Domain.Entities;

public sealed class Post
{
    public static class Limits
    {
        public const int MaxTitleLength = 100;
        public const int MaxSlugLength = 120;
        public const int MaxContentLength = 3000;
        public const int MaxImageUrlLength = 2048;
        public const int MaxDeleteReasonLength = 500;
    }

    private Post()
    {
    }

    public Guid Id { get; init; } = Guid.CreateVersion7();
    public User? PostedBy { get; private set; }
    public Guid? PostedById { get; private set; }
    public SubThread SubThread { get; init; } = null!;
    public Guid SubThreadId { get; init; }
    public string? Title { get; private set; }
    public string? Content { get; private set; }
    public string? ImageUrl { get; private set; }
    public string? DeleteReason { get; private set; }
    public int Score { get; private set; }
    public int CommentCount { get; private set; }
    public string? Slug { get; private set; }
    public bool IsDeleted { get; private set; }
    public DateTime PostedAt { get; init; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; private set; }
    public DateTime? DeletedAt { get; private set; }

    public IReadOnlyCollection<Comment> Comments => _comments.AsReadOnly();
    public IReadOnlyCollection<SavedPost> Saves => _saves.AsReadOnly();
    public IReadOnlyCollection<PostVote> Votes => _votes.AsReadOnly();

    private readonly List<Comment> _comments = [];
    private readonly List<SavedPost> _saves = [];
    private readonly List<PostVote> _votes = [];

    /// <summary>Creates a new post instance.</summary>
    /// <remarks>
    /// Possible error types:
    /// <list type="bullet">
    ///     <item><see cref="ErrorType.TitleEmpty"/></item>
    ///     <item><see cref="ErrorType.ContentEmpty"/></item>
    ///     <item><see cref="ErrorType.TitleTooLong"/></item>
    ///     <item><see cref="ErrorType.ContentTooLong"/></item>
    ///     <item><see cref="ErrorType.ImageUrlTooLong"/></item>
    /// </list>
    /// </remarks>
    public static Result<Post> Create(User postedBy, SubThread subThread, string title, string content,
        string? imageUrl = null)
    {
        if (string.IsNullOrWhiteSpace(title))
            return Result<Post>.Error("Title cannot be empty.", ErrorType.TitleEmpty);

        if (string.IsNullOrWhiteSpace(content))
            return Result<Post>.Error("Content cannot be empty.", ErrorType.ContentEmpty);

        if (title.Length > Limits.MaxTitleLength)
            return Result<Post>
                .Error($"Title cannot exceed {Limits.MaxTitleLength} characters.", ErrorType.TitleTooLong);

        if (content.Length > Limits.MaxContentLength)
            return Result<Post>
                .Error($"Content cannot exceed {Limits.MaxContentLength} characters.", ErrorType.ContentTooLong);

        if (imageUrl is { Length: > Limits.MaxImageUrlLength })
            return Result<Post>
                .Error($"Image URL cannot exceed {Limits.MaxImageUrlLength} characters.", ErrorType.ImageUrlTooLong);

        if (string.IsNullOrWhiteSpace(imageUrl))
            imageUrl = null;

        var post = new Post
        {
            PostedBy = postedBy,
            PostedById = postedBy.Id,
            SubThread = subThread,
            SubThreadId = subThread.Id,
            Title = title,
            Content = content,
            ImageUrl = imageUrl,
            Slug = title.GenerateSlug()
        };

        return Result<Post>.Success(post);
    }

    public void ApplyCommentCountDelta(int delta)
    {
        CommentCount += delta;
    }

    public void ApplyVoteDelta(int delta)
    {
        Score += delta;
    }

    /// <summary>Edits an existing post object.</summary>
    /// <remarks>
    /// Possible error types:
    /// <list type="bullet">
    ///     <item><see cref="ErrorType.ContentEmpty"/></item>
    ///     <item><see cref="ErrorType.ContentTooLong"/></item>
    ///     <item><see cref="ErrorType.ImageUrlTooLong"/></item>
    /// </list>
    /// </remarks>
    public Result Edit(string newContent, string? newImageUrl = null)
    {
        if (string.IsNullOrWhiteSpace(newContent))
            return Result.Error("Content cannot be empty.", ErrorType.ContentEmpty);

        if (newContent.Length > Limits.MaxContentLength)
            return Result.Error($"Content cannot exceed {Limits.MaxContentLength} characters.",
                ErrorType.ContentTooLong);

        if (newImageUrl is { Length: > Limits.MaxImageUrlLength })
            return Result.Error($"Image URL cannot exceed {Limits.MaxImageUrlLength} characters.",
                ErrorType.ImageUrlTooLong);

        if (string.IsNullOrWhiteSpace(newImageUrl))
            newImageUrl = null;

        Content = newContent;
        ImageUrl = newImageUrl;
        UpdatedAt = DateTime.UtcNow;

        return Result.Success();
    }

    /// <summary>Soft deletes a post.</summary>
    /// <remarks>
    /// Can be used by SubThread mods and site admins to mark posts for removal and include an optional reason for removal.
    /// <br/><br/>
    /// Possible error types:
    /// <list type="bullet">
    ///     <item><see cref="ErrorType.AlreadyDeleted"/></item>
    ///     <item><see cref="ErrorType.DeleteReasonTooLong"/></item>
    /// </list>
    /// </remarks>
    public Result SoftDelete(string? reason = null)
    {
        if (IsDeleted)
            return Result.Error("Post already deleted.", ErrorType.AlreadyDeleted);

        if (reason is { Length: > Limits.MaxDeleteReasonLength })
            return Result.Error($"Delete reason cannot exceed {Limits.MaxDeleteReasonLength} characters.",
                ErrorType.DeleteReasonTooLong);

        IsDeleted = true;
        DeleteReason = reason;
        Title = null;
        Content = null;
        Slug = null;
        DeletedAt = DateTime.UtcNow;
        _saves.Clear();

        return Result.Success();
    }
}

public sealed class PinnedSubThreadPost
{
    private PinnedSubThreadPost()
    {
    }

    public Guid Id { get; init; } = Guid.CreateVersion7();
    public required SubThread SubThread { get; init; }
    public Guid SubThreadId { get; init; }
    public Post Post { get; init; } = null!;
    public Guid PostId { get; init; }
    public User? PinnedBy { get; init; }
    public Guid? PinnedById { get; init; }
    public DateTime PinnedAt { get; init; } = DateTime.UtcNow;

    public static PinnedSubThreadPost Create(SubThread subThread, Post post, User user)
    {
        return new PinnedSubThreadPost
        {
            SubThread = subThread,
            SubThreadId = subThread.Id,
            Post = post,
            PostId = post.Id,
            PinnedBy = user,
            PinnedById = user.Id,
        };
    }
}

public sealed class SavedPost
{
    private SavedPost()
    {
    }

    public Guid Id { get; init; } = Guid.CreateVersion7();
    public Post Post { get; init; } = null!;
    public Guid PostId { get; init; }
    public User User { get; init; } = null!;
    public Guid UserId { get; init; }
    public DateTime SavedAt { get; init; } = DateTime.UtcNow;

    public static SavedPost Save(Post post, User user)
    {
        return new SavedPost
        {
            Post = post,
            PostId = post.Id,
            User = user,
            UserId = user.Id,
        };
    }
}

public sealed class PostVote
{
    private PostVote()
    {
    }

    public Guid Id { get; init; } = Guid.CreateVersion7();
    public Post Post { get; init; } = null!;
    public Guid PostId { get; init; }
    public User? User { get; init; }
    public Guid? UserId { get; init; }
    public DateTime VotedAt { get; private set; } = DateTime.UtcNow;
    public bool IsUpvote { get; private set; }
    public bool IsDownvote { get; private set; }

    public bool HasVoted => IsDownvote || IsUpvote;

    public static PostVote Create(Post post, User user, bool isUpvote)
    {
        return new PostVote
        {
            Post = post,
            PostId = post.Id,
            User = user,
            UserId = user.Id,
            IsUpvote = isUpvote,
            IsDownvote = !isUpvote,
        };
    }

    public void Upvote()
    {
        IsDownvote = false;
        IsUpvote = !IsUpvote;
        VotedAt = DateTime.UtcNow;
    }

    public void Downvote()
    {
        IsUpvote = false;
        IsDownvote = !IsDownvote;
        VotedAt = DateTime.UtcNow;
    }
}

public sealed class PostView
{
    private PostView()
    {
    }

    public Guid Id { get; init; } = Guid.CreateVersion7();
    public Post Post { get; init; } = null!;
    public Guid PostId { get; init; }
    public User User { get; init; } = null!;
    public Guid UserId { get; init; }
    public DateTime ViewedAt { get; private set; } = DateTime.UtcNow;

    public static PostView Create(Post post, User user)
    {
        return new PostView
        {
            Post = post,
            PostId = post.Id,
            User = user,
            UserId = user.Id,
        };
    }

    public void View()
    {
        ViewedAt = DateTime.UtcNow;
    }
}