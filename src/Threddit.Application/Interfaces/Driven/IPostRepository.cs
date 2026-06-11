using Threddit.Domain.Common;
using Threddit.Domain.Entities;

namespace Threddit.Application.Interfaces.Driven;

public interface IPostRepository
{
    /// <summary>Gets one post by GUID.</summary>
    /// <remarks>
    /// Possible error types:
    /// <list type="bullet">
    ///     <item><see cref="ErrorType.PostNotFound"/></item>
    ///     <item><see cref="ErrorType.DatabaseTimeout"/></item>
    /// </list>
    /// </remarks>
    Task<Result<Post>> GetByIdAsync(Guid postId);

    /// <summary>Gets one post by GUID along with its top level comments and one set of replies.</summary>
    /// <remarks>
    /// Possible error types:
    /// <list type="bullet">
    ///     <item><see cref="ErrorType.DatabaseTimeout"/></item>
    ///     <item><see cref="ErrorType.PostNotFound"/></item>
    /// </list>
    /// </remarks>
    Task<Result<Post>> GetByIdWithCommentsAsync(Guid postId);

    /// <summary>Saves a single post to the database.</summary>
    /// <remarks>
    /// Possible error types:
    /// <list type="bullet">
    ///     <item><see cref="ErrorType.DatabaseTimeout"/></item>
    ///     <item><see cref="ErrorType.ConcurrencyFailure"/></item>
    ///     <item><see cref="ErrorType.DatabaseUpdateFailure"/></item>
    /// </list>
    /// </remarks>
    Task<Result<Post>> CreateAsync(Post post);

    /// <summary>Takes in a post marked for deletion, scrubs its comments, and updates the database.</summary>
    /// <remarks>
    /// Possible error types:
    /// <list type="bullet">
    ///     <item><see cref="ErrorType.DatabaseTimeout"/></item>
    ///     <item><see cref="ErrorType.ConcurrencyFailure"/></item>
    ///     <item><see cref="ErrorType.DatabaseUpdateFailure"/></item>
    /// </list>
    /// </remarks>
    Task<Result> DeletePostAndRelatedDataAsync(Post post);

    /// <summary>Updates a single post in the database.</summary>
    /// <remarks>
    /// Possible error types:
    /// <list type="bullet">
    ///     <item><see cref="ErrorType.DatabaseTimeout"/></item>
    ///     <item><see cref="ErrorType.ConcurrencyFailure"/></item>
    ///     <item><see cref="ErrorType.DatabaseUpdateFailure"/></item>
    /// </list>
    /// </remarks>
    Task<Result> UpdateAsync(Post post);

    /// <summary>Gets an existing vote by a user on a post, or null if none exists.</summary>
    /// <remarks>
    /// Possible error types:
    /// <list type="bullet">
    ///     <item><see cref="ErrorType.DatabaseTimeout"/></item>
    /// </list>
    /// </remarks>
    Task<Result<PostVote?>> GetVoteAsync(Guid postId, Guid userId);

    /// <summary>Creates or updates a vote on a post in a database transaction.</summary>
    /// <remarks>
    /// Possible error types:
    /// <list type="bullet">
    ///     <item><see cref="ErrorType.DatabaseTimeout"/></item>
    ///     <item><see cref="ErrorType.ConcurrencyFailure"/></item>
    ///     <item><see cref="ErrorType.DatabaseUpdateFailure"/></item>
    /// </list>
    /// </remarks>
    Task<Result<int>> ProcessVoteAsync(Guid postId, Guid userId, bool isUpvote);
}