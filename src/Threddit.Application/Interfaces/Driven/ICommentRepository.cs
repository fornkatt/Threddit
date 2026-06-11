using Threddit.Domain.Common;
using Threddit.Domain.Entities;

namespace Threddit.Application.Interfaces.Driven;

public interface ICommentRepository
{
    /// <summary>Gets the replies to a specific comment.</summary>
    /// <remarks>
    /// Possible error types:
    /// <list type="bullet">
    ///     <item><see cref="ErrorType.DatabaseTimeout"/></item>
    /// </list>
    /// </remarks>
    Task<Result<IReadOnlyList<Comment>>> GetRepliesAsync(Guid parentCommentId);

    /// <summary>Gets one comment by GUID.</summary>
    /// <remarks>
    /// Possible error types:
    /// <list type="bullet">
    ///     <item><see cref="ErrorType.CommentNotFound"/></item>
    ///     <item><see cref="ErrorType.DatabaseTimeout"/></item>
    /// </list>
    /// </remarks>
    Task<Result<Comment>> GetByIdAsync(Guid commentId);

    /// <summary>Saves a comment to a specific post and updates the post's counter in a transaction to the database.</summary>
    /// <remarks>
    /// Possible error types:
    /// <list type="bullet">
    ///     <item><see cref="ErrorType.DatabaseTimeout"/></item>
    ///     <item><see cref="ErrorType.ConcurrencyFailure"/></item>
    ///     <item><see cref="ErrorType.DatabaseUpdateFailure"/></item>
    /// </list>
    /// </remarks>
    Task<Result<Comment>> CreateWithCounterUpdatesAsync(Comment comment, Post post, Comment? parentComment = null);

    /// <summary>Updates the specified comment in the database.</summary>
    /// <remarks>
    /// Possible error types:
    /// <list type="bullet">
    ///     <item><see cref="ErrorType.DatabaseTimeout"/></item>
    ///     <item><see cref="ErrorType.ConcurrencyFailure"/></item>
    ///     <item><see cref="ErrorType.DatabaseUpdateFailure"/></item>
    /// </list>
    /// </remarks>
    Task<Result<Comment>> UpdateAsync(Comment comment);

    /// <summary>Gets an existing vote by a user on a comment, or null if none exists.</summary>
    /// <remarks>
    /// Possible error types:
    /// <list type="bullet">
    ///     <item><see cref="ErrorType.DatabaseTimeout"/></item>
    /// </list>
    /// </remarks>
    Task<Result<CommentVote?>> GetVoteAsync(Guid commentId, Guid userId);

    /// <summary>Creates or updates a vote on a comment in a database transaction.</summary>
    /// <remarks>
    /// Possible error types:
    /// <list type="bullet">
    ///     <item><see cref="ErrorType.DatabaseTimeout"/></item>
    ///     <item><see cref="ErrorType.ConcurrencyFailure"/></item>
    ///     <item><see cref="ErrorType.DatabaseUpdateFailure"/></item>
    /// </list>
    /// </remarks>
    Task<Result<int>> ProcessVoteAsync(Guid commentId, Guid userId, bool isUpvote);
}