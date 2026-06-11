using System.Collections.Immutable;
using Threddit.Application.DTOs.Common;
using Threddit.Application.DTOs.Requests.SubThreads;
using Threddit.Domain.Common;
using Threddit.Domain.Entities;

namespace Threddit.Application.Interfaces.Driven;

public interface ISubThreadRepository
{
    /// <summary>Implements search logic for SubThreads that takes in a query string and pagination parameters.</summary>
    /// <remarks>
    /// Possible error types:
    /// <list type="bullet">
    ///     <item><see cref="ErrorType.DatabaseTimeout"/></item>
    /// </list>
    /// </remarks>
    Task<Result<PagedResult<SubThread>>> SearchAsync(string query, int page, int pageSize);
    
    /// <summary>Gets the top SubThreads, paged, based on subscriber count.</summary>
    /// <remarks>
    /// Possible error types:
    /// <list type="bullet">
    ///     <item><see cref="ErrorType.DatabaseTimeout"/></item>
    /// </list>
    /// </remarks>
    Task<Result<PagedResult<SubThread>>> GetTopAsync(int page, int pageSize);
    
    /// <summary>Gets a single SubThread by name.</summary>
    /// <remarks>
    /// Possible error types:
    /// <list type="bullet">
    ///     <item><see cref="ErrorType.SubThreadNotFound"/></item>
    ///     <item><see cref="ErrorType.DatabaseTimeout"/></item>
    /// </list>
    /// </remarks>
    Task<Result<SubThread>> GetByNameAsync(string name);
    
    /// <summary>Gets a SubThread's posts, paged, by the requested sort order.</summary>
    /// <remarks>
    /// Possible error types:
    /// <list type="bullet">
    ///     <item><see cref="ErrorType.DatabaseTimeout"/></item>
    /// </list>
    /// </remarks>
    Task<Result<PagedResult<Post>>> GetPostsAsync(Guid subThreadId, int page, int pageSize, PostSortOrder sortOrder);
    
    /// <summary>Gets a single SubThread by GUID.</summary>
    /// <remarks>
    /// Possible error types:
    /// <list type="bullet">
    ///     <item><see cref="ErrorType.SubThreadNotFound"/></item>
    ///     <item><see cref="ErrorType.DatabaseTimeout"/></item>
    /// </list>
    /// </remarks>
    Task<Result<SubThread>> GetByIdAsync(Guid subThreadId);
    
    /// <summary>Creates a new SubThread.</summary>
    /// <remarks>
    /// Possible error types:
    /// <list type="bullet">
    ///     <item><see cref="ErrorType.DatabaseUpdateFailure"/></item>
    ///     <item><see cref="ErrorType.ConcurrencyFailure"/></item>
    ///     <item><see cref="ErrorType.DatabaseTimeout"/></item>
    /// </list>
    /// </remarks>
    Task<Result<SubThread>> CreateAsync(SubThread subThread);
    
    /// <summary>Deletes a SubThread.</summary>
    /// <remarks>
    /// Possible error types:
    /// <list type="bullet">
    ///     <item><see cref="ErrorType.ConcurrencyFailure"/></item>
    ///     <item><see cref="ErrorType.DatabaseTimeout"/></item>
    ///     <item><see cref="ErrorType.DatabaseConstraint"/></item>
    /// </list>
    /// </remarks>
    Task<Result> DeleteAsync(SubThread subThread);
    
    /// <summary>Gets an existing subscription for a user on a SubThread, or null if none exists.</summary>
    /// <remarks>
    /// Possible error types:
    /// <list type="bullet">
    ///     <item><see cref="ErrorType.DatabaseTimeout"/></item>
    /// </list>
    /// </remarks>
    Task<Result<SubThreadSubscription?>> GetSubscriptionAsync(Guid subThreadId, Guid userId);
    
    /// <summary>Creates a subscription and increments the subscriber count in a database transaction.</summary>
    /// <remarks>
    /// Possible error types:
    /// <list type="bullet">
    ///     <item><see cref="ErrorType.DatabaseTimeout"/></item>
    ///     <item><see cref="ErrorType.ConcurrencyFailure"/></item>
    ///     <item><see cref="ErrorType.DatabaseUpdateFailure"/></item>
    /// </list>
    /// </remarks>
    Task<Result> SubscribeAsync(SubThreadSubscription subscription, SubThread subThread);

    /// <summary>Removes a subscription and decrements the subscriber count in a database transaction.</summary>
    /// <remarks>
    /// Possible error types:
    /// <list type="bullet">
    ///     <item><see cref="ErrorType.DatabaseTimeout"/></item>
    ///     <item><see cref="ErrorType.ConcurrencyFailure"/></item>
    ///     <item><see cref="ErrorType.DatabaseUpdateFailure"/></item>
    /// </list>
    /// </remarks>
    Task<Result> UnsubscribeAsync(SubThreadSubscription subscription, SubThread subThread);
    
    /// <summary>Updates a single SubThread in the database.</summary>
    /// <remarks>
    /// Possible error types:
    /// <list type="bullet">
    ///     <item><see cref="ErrorType.ConcurrencyFailure"/></item>
    ///     <item><see cref="ErrorType.DatabaseUpdateFailure"/></item>
    ///     <item><see cref="ErrorType.DatabaseTimeout"/></item>
    /// </list>
    /// </remarks>
    Task<Result> UpdateAsync(SubThread subThread);
    
    /// <summary>Gets a single SubThread rule by GUID.</summary>
    /// <remarks>
    /// Possible error types:
    /// <list type="bullet">
    ///     <item><see cref="ErrorType.SubThreadRuleNotFound"/></item>
    ///     <item><see cref="ErrorType.DatabaseTimeout"/></item>
    /// </list>
    /// </remarks>
    Task<Result<SubThreadRule>> GetRuleByIdAsync(Guid ruleId);
    
    /// <summary>Adds a new rule to a SubThread.</summary>
    /// <remarks>
    /// Possible error types:
    /// <list type="bullet">
    ///     <item><see cref="ErrorType.DatabaseUpdateFailure"/></item>
    ///     <item><see cref="ErrorType.ConcurrencyFailure"/></item>
    ///     <item><see cref="ErrorType.DatabaseTimeout"/></item>
    /// </list>
    /// </remarks>
    Task<Result<SubThreadRule>> AddRuleAsync(SubThreadRule rule);
    
    /// <summary>Updates an existing SubThread rule in the database.</summary>
    /// <remarks>
    /// Possible error types:
    /// <list type="bullet">
    ///     <item><see cref="ErrorType.ConcurrencyFailure"/></item>
    ///     <item><see cref="ErrorType.DatabaseUpdateFailure"/></item>
    ///     <item><see cref="ErrorType.DatabaseTimeout"/></item>
    /// </list>
    /// </remarks>
    Task<Result> UpdateRuleAsync(SubThreadRule rule);
    
    /// <summary>Deletes a SubThread rule.</summary>
    /// <remarks>
    /// Possible error types:
    /// <list type="bullet">
    ///     <item><see cref="ErrorType.ConcurrencyFailure"/></item>
    ///     <item><see cref="ErrorType.DatabaseConstraint"/></item>
    ///     <item><see cref="ErrorType.DatabaseTimeout"/></item>
    /// </list>
    /// </remarks>
    Task<Result> DeleteRuleAsync(SubThreadRule rule);
    
    /// <summary>Gets an existing moderator from a SubThread, or null if none exists.</summary>
    /// <remarks>
    /// Possible error types:
    /// <list type="bullet">
    ///     <item><see cref="ErrorType.DatabaseTimeout"/></item>
    /// </list>
    /// </remarks>
    Task<Result<SubThreadModerator?>> GetModeratorAsync(Guid subThreadId, Guid userId);
    
    /// <summary>Adds a new moderator to a SubThread.</summary>
    /// <remarks>
    /// Possible error types:
    /// <list type="bullet">
    ///     <item><see cref="ErrorType.ConcurrencyFailure"/></item>
    ///     <item><see cref="ErrorType.DatabaseUpdateFailure"/></item>
    ///     <item><see cref="ErrorType.DatabaseTimeout"/></item>
    /// </list>
    /// </remarks>
    Task<Result> AddModeratorAsync(SubThreadModerator moderator);
    
    /// <summary>Removes a moderator from a SubThread.</summary>
    /// <remarks>
    /// Possible error types:
    /// <list type="bullet">
    ///     <item><see cref="ErrorType.ConcurrencyFailure"/></item>
    ///     <item><see cref="ErrorType.DatabaseConstraint"/></item>
    ///     <item><see cref="ErrorType.DatabaseTimeout"/></item>
    /// </list>
    /// </remarks>
    Task<Result> RemoveModeratorAsync(SubThreadModerator moderator);
    
    /// <summary>Gets all moderators of a SubThread, including their user details.</summary>
    /// <remarks>
    /// Possible error types:
    /// <list type="bullet">
    ///     <item><see cref="ErrorType.SubThreadNotFound"/></item>
    ///     <item><see cref="ErrorType.DatabaseTimeout"/></item>
    /// </list>
    /// </remarks>
    Task<Result<ImmutableList<SubThreadModerator>>> GetModeratorsAsync(string subThreadName);
}