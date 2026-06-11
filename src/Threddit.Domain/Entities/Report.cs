using Threddit.Domain.Common;

namespace Threddit.Domain.Entities;

public sealed class Report
{
    public static class Limits
    {
        public const int MaxMessageLength = 500;
    }

    private Report()
    {
    }

    public Guid Id { get; init; } = Guid.CreateVersion7();
    public ReportStatus Status { get; private set; }
    public ReportCategory Category { get; init; }
    public ReportType Type { get; init; }
    public string? Message { get; init; }

    public User? Reporter { get; init; }
    public Guid? ReporterId { get; init; }

    // Reports confined to a SubThread
    public SubThread? SubThread { get; init; }
    public Guid? SubThreadId { get; init; }
    public Post? TargetPost { get; init; }
    public Guid? TargetPostId { get; init; }

    // It helps if we can track the post that the comment belongs to
    public Post? Post { get; init; }
    public Guid? PostId { get; init; }
    public Comment? TargetComment { get; init; }
    public Guid? TargetCommentId { get; init; }

    // Site-wide reports that admins handle
    public User? TargetUser { get; init; }
    public Guid? TargetUserId { get; init; }
    public SubThread? TargetSubThread { get; init; }
    public Guid? TargetSubThreadId { get; init; }
    public DirectMessage? TargetDirectMessage { get; init; }
    public Guid? TargetDirectMessageId { get; init; }

    public DateTime ReportedAt { get; init; } = DateTime.UtcNow;

    /// <summary>Creates a new report instance for posts.</summary>
    /// <remarks>
    /// Possible error types:
    /// <list type="bullet">
    ///     <item><see cref="ErrorType.PostDoesNotBelongToSubThread"/></item>
    ///     <item><see cref="ErrorType.ContentTooLong"/></item>
    /// </list>
    /// </remarks>
    public static Result<Report> ForPost(User reporter, Post post, Guid subThreadId, ReportCategory category,
        string? message = null)
    {
        if (post.SubThreadId != subThreadId)
            return Result<Report>.Error("The provided post does not belong to the SubThread provided",
                ErrorType.PostDoesNotBelongToSubThread);

        if (message is { Length: > Limits.MaxMessageLength })
            return Result<Report>.Error($"Report message cannot exceed {Limits.MaxMessageLength} characters.",
                ErrorType.ContentTooLong);

        return Result<Report>.Success(new Report()
        {
            Type = ReportType.Post,
            Reporter = reporter,
            ReporterId = reporter.Id,
            SubThreadId = subThreadId,
            TargetPost = post,
            TargetPostId = post.Id,
            Category = category,
            Message = message,
            Status = ReportStatus.Pending
        });
    }

    /// <summary>Creates a new report instance for comments.</summary>
    /// <remarks>
    /// Possible error types:
    /// <list type="bullet">
    ///     <item><see cref="ErrorType.CommentDoesNotBelongToPost"/></item>
    ///     <item><see cref="ErrorType.PostDoesNotBelongToSubThread"/></item>
    ///     <item><see cref="ErrorType.ContentTooLong"/></item>
    /// </list>
    /// </remarks>
    public static Result<Report> ForComment(User reporter, Comment targetComment, Guid postId, Guid subThreadId,
        ReportCategory category,
        string? message = null)
    {
        if (targetComment.PostId != postId)
            return Result<Report>.Error("The provided comment does not belong to the post provided",
                ErrorType.CommentDoesNotBelongToPost);

        if (targetComment.SubThreadId != subThreadId)
            return Result<Report>.Error("The provided post does not belong to the SubThread provided",
                ErrorType.PostDoesNotBelongToSubThread);

        if (message is { Length: > Limits.MaxMessageLength })
            return Result<Report>
                .Error($"Report message cannot exceed {Limits.MaxMessageLength} characters.", ErrorType.ContentTooLong);

        return Result<Report>.Success(new Report
        {
            Type = ReportType.Comment,
            Reporter = reporter,
            ReporterId = reporter.Id,
            PostId = postId,
            SubThreadId = subThreadId,
            TargetComment = targetComment,
            TargetCommentId = targetComment.Id,
            Category = category,
            Message = message,
            Status = ReportStatus.Pending
        });
    }

    /// <summary>Creates a new report instance for users.</summary>
    /// <remarks>
    /// Possible error types:
    /// <list type="bullet">
    ///     <item><see cref="ErrorType.ContentTooLong"/></item>
    /// </list>
    /// </remarks>
    public static Result<Report> ForUser(User reporter, User user, ReportCategory category, string? message = null)
    {
        if (message is { Length: > Limits.MaxMessageLength })
            return Result<Report>
                .Error($"Report message cannot exceed {Limits.MaxMessageLength} characters.", ErrorType.ContentTooLong);

        return Result<Report>.Success(new Report
        {
            Type = ReportType.User,
            Reporter = reporter,
            ReporterId = reporter.Id,
            TargetUser = user,
            TargetUserId = user.Id,
            Category = category,
            Message = message,
            Status = ReportStatus.Pending
        });
    }

    /// <summary>Creates a new report instance for SubThreads.</summary>
    /// <remarks>
    /// Possible error types:
    /// <list type="bullet">
    ///     <item><see cref="ErrorType.ContentTooLong"/></item>
    /// </list>
    /// </remarks>
    public static Result<Report> ForSubThread(User reporter, SubThread targetSubThread, ReportCategory category,
        string? message = null)
    {
        if (message is { Length: > Limits.MaxMessageLength })
            return Result<Report>
                .Error($"Report message cannot exceed {Limits.MaxMessageLength} characters.", ErrorType.ContentTooLong);
        
        return Result<Report>.Success(new Report
        {
            Type = ReportType.SubThread,
            Reporter = reporter,
            ReporterId = reporter.Id,
            TargetSubThread = targetSubThread,
            TargetSubThreadId = targetSubThread.Id,
            Category = category,
            Message = message,
            Status = ReportStatus.Pending
        });
    }

    /// <summary>Creates a new report instance for direct messages.</summary>
    /// <remarks>
    /// Possible error types:
    /// <list type="bullet">
    ///     <item><see cref="ErrorType.ContentTooLong"/></item>
    /// </list>
    /// </remarks>
    public static Result<Report> ForDirectMessage(User reporter, DirectMessage targetDirectMessage, ReportCategory category,
        string? message = null)
    {
        if (message is { Length: > Limits.MaxMessageLength })
            return Result<Report>
                .Error($"Report message cannot exceed {Limits.MaxMessageLength} characters.", ErrorType.ContentTooLong);

        return Result<Report>.Success(new Report
        {
            Type = ReportType.DirectMessage,
            Reporter = reporter,
            ReporterId = reporter.Id,
            TargetDirectMessage = targetDirectMessage,
            TargetDirectMessageId = targetDirectMessage.Id,
            Category = category,
            Message = message,
            Status = ReportStatus.Pending
        });
    }

    /// <summary>Sets the status of an existing report.</summary>
    /// <remarks>
    /// Possible error types:
    /// <list type="bullet">
    ///     <item><see cref="ErrorType.SameStatus"/></item>
    /// </list>
    /// </remarks>
    public Result SetStatus(ReportStatus status)
    {
        if (Status == status)
            return Result.Error("Status cannot be the same as the current status", ErrorType.SameStatus);

        Status = status;

        return Result.Success();
    }

    public enum ReportType
    {
        Post,
        Comment,
        User,
        SubThread,
        DirectMessage,
    }

    public enum ReportStatus
    {
        Pending,
        Received,
        Dismissed,
        Resolved
    }

    public enum ReportCategory
    {
        Spam,
        Harassment,
        RuleViolation,
        Other
    }
}