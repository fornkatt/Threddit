using Threddit.Domain.Common;

namespace Threddit.Domain.Entities;

public sealed class Comment
{
    public static class Limits
    {
        public const int MaxContentLength = 3000;
        public const int MaxDeleteReasonLength = 500;
        public const int MaxImageUrlLength = 2048;
    }

    private Comment()
    {
    }

    public Guid Id { get; init; } = Guid.CreateVersion7();
    public User? CommentedBy { get; private set; }
    public Guid? CommentedById { get; private set; }
    public Post Post { get; init; } = null!;
    public Guid PostId { get; init; }
    public Guid SubThreadId { get; init; }
    public string? Content { get; private set; }
    public string? ImageUrl { get; private set; }
    public string? DeleteReason { get; private set; }
    public int Score { get; private set; }
    public int ReplyCount { get; private set; }
    public DateTime CommentedAt { get; init; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; private set; }
    public DateTime? DeletedAt { get; private set; }
    public Comment? ParentComment { get; init; }
    public Guid? ParentCommentId { get; init; }

    public bool IsDeleted { get; private set; }

    public IReadOnlyCollection<Comment> Replies => _replies.AsReadOnly();
    public IReadOnlyCollection<CommentVote> Votes => _votes.AsReadOnly();
    public IReadOnlyCollection<SavedComment> Saves => _saves.AsReadOnly();

    private readonly List<Comment> _replies = [];
    private readonly List<CommentVote> _votes = [];
    private readonly List<SavedComment> _saves = [];

    /// <summary>Creates a new comment instance.</summary>
    /// <remarks>
    /// Possible error types:
    /// <list type="bullet">
    ///     <item><see cref="ErrorType.ContentEmpty"/></item>
    ///     <item><see cref="ErrorType.ContentTooLong"/></item>
    /// </list>
    /// </remarks>
    public static Result<Comment> Create(User user, Post post, string content, Comment? parentComment = null,
        string? imageUrl = null)
    {
        if (string.IsNullOrWhiteSpace(content))
            return Result<Comment>.Error("Content cannot be empty.", ErrorType.ContentEmpty);

        if (content.Length > Limits.MaxContentLength)
            return Result<Comment>.Error($"Content cannot exceed {Limits.MaxContentLength} characters.",
                ErrorType.ContentTooLong);

        if (imageUrl is { Length: > Limits.MaxImageUrlLength })
            return Result<Comment>.Error($"Image URL cannot exceed {Limits.MaxImageUrlLength} characters.",
                ErrorType.ImageUrlTooLong);
        
        if (string.IsNullOrWhiteSpace(imageUrl))
            imageUrl = null;

        return Result<Comment>.Success(new Comment
        {
            CommentedBy = user,
            CommentedById = user.Id,
            Post = post,
            PostId = post.Id,
            SubThreadId = post.SubThreadId,
            Content = content,
            ImageUrl = imageUrl,
            ParentComment = parentComment,
            ParentCommentId = parentComment?.Id
        });
    }

    public void ApplyVoteDelta(int delta)
    {
        Score += delta;
    }

    public void ApplyReplyCountDelta(int delta)
    {
        ReplyCount += delta;
    }

    /// <summary>Edits an existing comment object.</summary>
    /// <remarks>
    /// Possible error types:
    /// <list type="bullet">
    ///     <item><see cref="ErrorType.AlreadyDeleted"/></item>
    ///     <item><see cref="ErrorType.ContentEmpty"/></item>
    ///     <item><see cref="ErrorType.ContentTooLong"/></item>
    /// </list>
    /// </remarks>
    public Result Edit(string newContent, string? newImageUrl = null)
    {
        if (IsDeleted)
            return Result.Error("Cannot edit a deleted comment.", ErrorType.AlreadyDeleted);

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

    /// <summary>Soft deletes a comment.</summary>
    /// <remarks>
    /// To be used instead of hard deleting a comment in the database to preserve thread integrity.
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
            return Result.Error("Comment already deleted.", ErrorType.AlreadyDeleted);

        if (reason is { Length: > Limits.MaxDeleteReasonLength })
            return Result.Error($"Delete reason cannot exceed {Limits.MaxDeleteReasonLength} characters.",
                ErrorType.DeleteReasonTooLong);

        IsDeleted = true;
        DeleteReason = reason;
        Content = null;
        ImageUrl = null;
        DeletedAt = DateTime.UtcNow;
        _saves.Clear();

        return Result.Success();
    }
}

public sealed class SavedComment
{
    private SavedComment()
    {
    }

    public Guid Id { get; init; } = Guid.CreateVersion7();
    public Comment Comment { get; init; } = null!;
    public Guid CommentId { get; init; }
    public User User { get; init; } = null!;
    public Guid UserId { get; init; }
    public DateTime SavedAt { get; init; } = DateTime.UtcNow;

    public static SavedComment Create(Comment comment, User user)
    {
        return new SavedComment()
        {
            Comment = comment,
            CommentId = comment.Id,
            User = user,
            UserId = user.Id,
        };
    }
}

public sealed class CommentVote
{
    private CommentVote()
    {
    }

    public Guid Id { get; init; } = Guid.CreateVersion7();
    public Comment Comment { get; init; } = null!;
    public Guid CommentId { get; init; }
    public User? User { get; init; }
    public Guid? UserId { get; init; }
    public DateTime VotedAt { get; private set; } = DateTime.UtcNow;
    public bool IsUpvote { get; private set; }
    public bool IsDownvote { get; private set; }

    public bool HasVoted => IsDownvote || IsUpvote;

    public static CommentVote Create(Comment comment, User user, bool isUpvote)
    {
        return new CommentVote
        {
            Comment = comment,
            CommentId = comment.Id,
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