using System.Collections.Immutable;
using Threddit.Domain.Common;
using Threddit.Domain.Entities;

namespace Threddit.Application.Interfaces.Driven;

public interface IUserRepository
{
    /// <summary>Gets a single user by GUID.</summary>
    /// <remarks>
    /// Possible error types:
    /// <list type="bullet">
    ///     <item><see cref="ErrorType.UserNotFound"/></item>
    ///     <item><see cref="ErrorType.DatabaseTimeout"/></item>
    /// </list>
    /// </remarks>
    Task<Result<User>> GetByIdAsync(Guid userId);
    
    /// <summary>Gets a single user by username.</summary>
    /// <remarks>
    /// Possible error types:
    /// <list type="bullet">
    ///     <item><see cref="ErrorType.UserNotFound"/></item>
    ///     <item><see cref="ErrorType.DatabaseTimeout"/></item>
    /// </list>
    /// </remarks>
    Task<Result<User>> GetByUsernameAsync(string username);
    
    /// <summary>Gets a single user by email.</summary>
    /// <remarks>
    /// Possible error types:
    /// <list type="bullet">
    ///     <item><see cref="ErrorType.UserNotFound"/></item>
    ///     <item><see cref="ErrorType.DatabaseTimeout"/></item>
    /// </list>
    /// </remarks>
    Task<Result<User>> GetByEmailAsync(string email);
    
    /// <summary>Gets a single user along with all roles by GUID.</summary>
    /// <remarks>
    /// Possible error types:
    /// <list type="bullet">
    ///     <item><see cref="ErrorType.UserNotFound"/></item>
    ///     <item><see cref="ErrorType.DatabaseTimeout"/></item>
    /// </list>
    /// </remarks>
    Task<Result<User>> GetByIdWithRolesAsync(Guid userId);

    /// <summary>Gets an existing site ban for a user, or null if none exists.</summary>
    /// <remarks>
    /// Possible error types:
    /// <list type="bullet">
    ///     <item><see cref="ErrorType.DatabaseTimeout"/></item>
    /// </list>
    /// </remarks>
    Task<Result<BannedSiteUser?>> GetSiteBanAsync(Guid userId);
    
    /// <summary>Creates a site-wide ban for a user.</summary>
    /// <remarks>
    /// Possible error types:
    /// <list type="bullet">
    ///     <item><see cref="ErrorType.DatabaseUpdateFailure"/></item>
    ///     <item><see cref="ErrorType.ConcurrencyFailure"/></item>
    ///     <item><see cref="ErrorType.DatabaseTimeout"/></item>
    /// </list>
    /// </remarks>
    Task<Result> AddSiteBanAsync(BannedSiteUser user);
    
    /// <summary>Updates an existing site ban in the database.</summary>
    /// <remarks>
    /// Possible error types:
    /// <list type="bullet">
    ///     <item><see cref="ErrorType.ConcurrencyFailure"/></item>
    ///     <item><see cref="ErrorType.DatabaseUpdateFailure"/></item>
    ///     <item><see cref="ErrorType.DatabaseTimeout"/></item>
    /// </list>
    /// </remarks>
    Task<Result> UpdateSiteBanAsync(BannedSiteUser user);
    
    /// <summary>Removes a site-wide ban for a user.</summary>
    /// <remarks>
    /// Possible error types:
    /// <list type="bullet">
    ///     <item><see cref="ErrorType.ConcurrencyFailure"/></item>
    ///     <item><see cref="ErrorType.DatabaseUpdateFailure"/></item>
    ///     <item><see cref="ErrorType.DatabaseTimeout"/></item>
    /// </list>
    /// </remarks>
    Task<Result> RemoveSiteBanAsync(BannedSiteUser user);
    
    /// <summary>Gets an existing SubThread ban for a user in a specific SubThread, or null if none exists.</summary>
    /// <remarks>
    /// Possible error types:
    /// <list type="bullet">
    ///     <item><see cref="ErrorType.DatabaseTimeout"/></item>
    /// </list>
    /// </remarks>
    Task<Result<BannedSubThreadUser?>> GetSubThreadBanAsync(Guid userId, Guid subThreadId);
    
    /// <summary>Creates a SubThread ban for a user.</summary>
    /// <remarks>
    /// Possible error types:
    /// <list type="bullet">
    ///     <item><see cref="ErrorType.DatabaseUpdateFailure"/></item>
    ///     <item><see cref="ErrorType.ConcurrencyFailure"/></item>
    ///     <item><see cref="ErrorType.DatabaseTimeout"/></item>
    /// </list>
    /// </remarks>
    Task<Result> AddSubThreadBanAsync(BannedSubThreadUser user);
    
    /// <summary>Updates an existing SubThread ban in the database.</summary>
    /// <remarks>
    /// Possible error types:
    /// <list type="bullet">
    ///     <item><see cref="ErrorType.ConcurrencyFailure"/></item>
    ///     <item><see cref="ErrorType.DatabaseUpdateFailure"/></item>
    ///     <item><see cref="ErrorType.DatabaseTimeout"/></item>
    /// </list>
    /// </remarks>
    Task<Result> UpdateSubThreadBanAsync(BannedSubThreadUser user);
    
    /// <summary>Removes a SubThread ban for a user.</summary>
    /// <remarks>
    /// Possible error types:
    /// <list type="bullet">
    ///     <item><see cref="ErrorType.ConcurrencyFailure"/></item>
    ///     <item><see cref="ErrorType.DatabaseUpdateFailure"/></item>
    ///     <item><see cref="ErrorType.DatabaseTimeout"/></item>
    /// </list>
    /// </remarks>
    Task<Result> RemoveSubThreadBanAsync(BannedSubThreadUser user);
    
    /// <summary>Updates an existing user in the database.</summary>
    /// <remarks>
    /// Primarily used to update the user's profile.
    /// <br/><br/>
    /// Possible error types:
    /// <list type="bullet">
    ///     <item><see cref="ErrorType.ConcurrencyFailure"/></item>
    ///     <item><see cref="ErrorType.DatabaseUpdateFailure"/></item>
    ///     <item><see cref="ErrorType.DatabaseTimeout"/></item>
    /// </list>
    /// </remarks>
    Task<Result> UpdateAsync(User user);
    
    /// <summary>Gets an existing site admin record by GUID.</summary>
    /// <remarks>
    /// Possible error types:
    /// <list type="bullet">
    ///     <item><see cref="ErrorType.SiteAdminNotFound"/></item>
    ///     <item><see cref="ErrorType.DatabaseTimeout"/></item>
    /// </list>
    /// </remarks>
    Task<Result<SiteAdmin>> GetSiteAdminAsync(Guid userId);
    
    /// <summary>Gets all site administrators with their user details and issued site bans.</summary>
    /// <remarks>
    /// Possible error types:
    /// <list type="bullet">
    ///     <item><see cref="ErrorType.DatabaseTimeout"/></item>
    /// </list>
    /// </remarks>
    Task<Result<ImmutableList<SiteAdmin>>> GetSiteAdminsAsync();
    
    /// <summary>Adds a site admin record to a user.</summary>
    /// <remarks>
    /// Possible error types:
    /// <list type="bullet">
    ///     <item><see cref="ErrorType.AlreadyAssigned"/></item>
    ///     <item><see cref="ErrorType.ConcurrencyFailure"/></item>
    ///     <item><see cref="ErrorType.DatabaseUpdateFailure"/></item>
    ///     <item><see cref="ErrorType.DatabaseTimeout"/></item>
    /// </list>
    /// </remarks>
    Task<Result> AddSiteAdminAsync(SiteAdmin siteAdmin);
    
    /// <summary>Removes a site admin record from a user.</summary>
    /// <remarks>
    /// Possible error types:
    /// <list type="bullet">
    ///     <item><see cref="ErrorType.ConcurrencyFailure"/></item>
    ///     <item><see cref="ErrorType.DatabaseTimeout"/></item>
    /// </list>
    /// </remarks>
    Task<Result> RemoveSiteAdminAsync(SiteAdmin siteAdmin);
    
    /// <summary>Gets the current user with all data needed for populating the client side Singleton store.</summary>
    /// <remarks>
    /// Possible error types:
    /// <list type="bullet">
    ///     <item><see cref="ErrorType.UserNotFound"/></item>
    ///     <item><see cref="ErrorType.DatabaseTimeout"/></item>
    /// </list>
    /// </remarks>
    Task<Result<User>> GetCurrentUserSummaryAsync(Guid userId);
}