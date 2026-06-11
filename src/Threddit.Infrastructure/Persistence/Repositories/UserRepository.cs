using System.Collections.Immutable;
using Microsoft.EntityFrameworkCore;
using Threddit.Application.Interfaces.Driven;
using Threddit.Domain.Common;
using Threddit.Domain.Entities;
using Threddit.Infrastructure.Persistence.Repositories.Common;

namespace Threddit.Infrastructure.Persistence.Repositories;

public sealed class UserRepository : IUserRepository
{
    private readonly ThredditDbContext _context;

    public UserRepository(
        ThredditDbContext context
    )
    {
        _context = context;
    }

    public async Task<Result<User>> GetByIdAsync(Guid userId)
    {
        try
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Id == userId);

            return user is null
                ? Result<User>.Error($"User not found: {userId}", ErrorType.UserNotFound)
                : Result<User>.Success(user);
        }
        catch (OperationCanceledException ex)
        {
            return Result<User>.Error(RepositoryMessageConstants.DatabaseTimeoutErrorMessage,
                ErrorType.DatabaseTimeout, ex);
        }
    }

    public async Task<Result<User>> GetByUsernameAsync(string username)
    {
        try
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.UserName == username);

            return user is null
                ? Result<User>.Error($"User not found: {username}", ErrorType.UserNotFound)
                : Result<User>.Success(user);
        }
        catch (OperationCanceledException ex)
        {
            return Result<User>.Error(RepositoryMessageConstants.DatabaseTimeoutErrorMessage,
                ErrorType.DatabaseTimeout, ex);
        }
    }

    public async Task<Result<User>> GetByEmailAsync(string email)
    {
        try
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == email);

            return user is null
                ? Result<User>.Error($"User not found: {email}", ErrorType.UserNotFound)
                : Result<User>.Success(user);
        }
        catch (OperationCanceledException ex)
        {
            return Result<User>.Error(RepositoryMessageConstants.DatabaseTimeoutErrorMessage,
                ErrorType.DatabaseTimeout, ex);
        }
    }

    public async Task<Result<User>> GetByIdWithRolesAsync(Guid userId)
    {
        try
        {
            var user = await _context.Users
                .Include(u => u.SiteOwner)
                .Include(u => u.SiteAdmin)
                .Include(u => u.BannedSiteUser)
                .Include(u => u.SubThreadModeratorRoles)
                .Include(u => u.ReceivedSubThreadBans)
                .FirstOrDefaultAsync(u => u.Id == userId);

            return user is null
                ? Result<User>.Error($"User not found: {userId}", ErrorType.UserNotFound)
                : Result<User>.Success(user);
        }
        catch (OperationCanceledException ex)
        {
            return Result<User>.Error(RepositoryMessageConstants.DatabaseTimeoutErrorMessage,
                ErrorType.DatabaseTimeout, ex);
        }
    }

    public async Task<Result<BannedSiteUser?>> GetSiteBanAsync(Guid userId)
    {
        try
        {
            return Result<BannedSiteUser?>.Success(await _context.BannedSiteUsers
                .FirstOrDefaultAsync(b => b.UserId == userId));
        }
        catch (OperationCanceledException ex)
        {
            return Result<BannedSiteUser?>.Error(RepositoryMessageConstants.DatabaseTimeoutErrorMessage,
                ErrorType.DatabaseTimeout, ex);
        }
    }

    public async Task<Result> AddSiteBanAsync(BannedSiteUser user)
    {
        try
        {
            _context.BannedSiteUsers.Add(user);
            await _context.SaveChangesAsync();

            return Result.Success();
        }
        catch (OperationCanceledException ex)
        {
            return Result.Error(RepositoryMessageConstants.DatabaseTimeoutErrorMessage, ErrorType.DatabaseTimeout, ex);
        }
        catch (DbUpdateConcurrencyException ex)
        {
            return Result.Error(RepositoryMessageConstants.ConcurrencyFailureErrorMessage,
                ErrorType.ConcurrencyFailure, ex);
        }
        catch (DbUpdateException ex)
        {
            return Result.Error("Database error while adding site ban.", ErrorType.DatabaseUpdateFailure, ex);
        }
    }

    public async Task<Result> UpdateSiteBanAsync(BannedSiteUser user)
    {
        try
        {
            _context.BannedSiteUsers.Update(user);
            await _context.SaveChangesAsync();

            return Result.Success();
        }
        catch (OperationCanceledException ex)
        {
            return Result.Error(RepositoryMessageConstants.DatabaseTimeoutErrorMessage, ErrorType.DatabaseTimeout, ex);
        }
        catch (DbUpdateConcurrencyException ex)
        {
            return Result.Error(RepositoryMessageConstants.ConcurrencyFailureErrorMessage,
                ErrorType.ConcurrencyFailure, ex);
        }
        catch (DbUpdateException ex)
        {
            return Result.Error("Database error while updating site ban.", ErrorType.DatabaseUpdateFailure, ex);
        }
    }

    public async Task<Result> RemoveSiteBanAsync(BannedSiteUser user)
    {
        try
        {
            _context.BannedSiteUsers.Remove(user);
            await _context.SaveChangesAsync();

            return Result.Success();
        }
        catch (OperationCanceledException ex)
        {
            return Result.Error(RepositoryMessageConstants.DatabaseTimeoutErrorMessage, ErrorType.DatabaseTimeout, ex);
        }
        catch (DbUpdateConcurrencyException ex)
        {
            return Result.Error(RepositoryMessageConstants.ConcurrencyFailureErrorMessage,
                ErrorType.ConcurrencyFailure, ex);
        }
        catch (DbUpdateException ex)
        {
            return Result.Error("Database error while removing site ban.", ErrorType.DatabaseUpdateFailure, ex);
        }
    }

    public async Task<Result<BannedSubThreadUser?>> GetSubThreadBanAsync(Guid userId, Guid subThreadId)
    {
        try
        {
            return Result<BannedSubThreadUser?>.Success(
                await _context.BannedSubThreadUsers
                    .FirstOrDefaultAsync(b => b.UserId == userId && b.SubThreadId == subThreadId));
        }
        catch (OperationCanceledException ex)
        {
            return Result<BannedSubThreadUser?>.Error(RepositoryMessageConstants.DatabaseTimeoutErrorMessage,
                ErrorType.DatabaseTimeout, ex);
        }
    }

    public async Task<Result> AddSubThreadBanAsync(BannedSubThreadUser user)
    {
        try
        {
            _context.BannedSubThreadUsers.Add(user);
            await _context.SaveChangesAsync();

            return Result.Success();
        }
        catch (OperationCanceledException ex)
        {
            return Result.Error(RepositoryMessageConstants.DatabaseTimeoutErrorMessage, ErrorType.DatabaseTimeout, ex);
        }
        catch (DbUpdateConcurrencyException ex)
        {
            return Result.Error(RepositoryMessageConstants.ConcurrencyFailureErrorMessage,
                ErrorType.ConcurrencyFailure, ex);
        }
        catch (DbUpdateException ex)
        {
            return Result.Error("Database error while adding SubThread ban.", ErrorType.DatabaseUpdateFailure, ex);
        }
    }

    public async Task<Result> UpdateSubThreadBanAsync(BannedSubThreadUser user)
    {
        try
        {
            _context.BannedSubThreadUsers.Update(user);
            await _context.SaveChangesAsync();

            return Result.Success();
        }
        catch (OperationCanceledException ex)
        {
            return Result.Error(RepositoryMessageConstants.DatabaseTimeoutErrorMessage, ErrorType.DatabaseTimeout, ex);
        }
        catch (DbUpdateConcurrencyException ex)
        {
            return Result.Error(RepositoryMessageConstants.ConcurrencyFailureErrorMessage,
                ErrorType.ConcurrencyFailure, ex);
        }
        catch (DbUpdateException ex)
        {
            return Result.Error("Database error while updating SubThread ban.", ErrorType.DatabaseUpdateFailure, ex);
        }
    }

    public async Task<Result> RemoveSubThreadBanAsync(BannedSubThreadUser user)
    {
        try
        {
            _context.BannedSubThreadUsers.Remove(user);
            await _context.SaveChangesAsync();

            return Result.Success();
        }
        catch (OperationCanceledException ex)
        {
            return Result.Error(RepositoryMessageConstants.DatabaseTimeoutErrorMessage, ErrorType.DatabaseTimeout, ex);
        }
        catch (DbUpdateConcurrencyException ex)
        {
            return Result.Error(RepositoryMessageConstants.ConcurrencyFailureErrorMessage,
                ErrorType.ConcurrencyFailure, ex);
        }
        catch (DbUpdateException ex)
        {
            return Result.Error("Database error while removing SubThread ban.", ErrorType.DatabaseUpdateFailure, ex);
        }
    }

    public async Task<Result> UpdateAsync(User user)
    {
        try
        {
            _context.Users.Update(user);
            await _context.SaveChangesAsync();

            return Result.Success();
        }
        catch (OperationCanceledException ex)
        {
            return Result.Error(RepositoryMessageConstants.DatabaseTimeoutErrorMessage, ErrorType.DatabaseTimeout, ex);
        }
        catch (DbUpdateConcurrencyException ex)
        {
            return Result.Error(RepositoryMessageConstants.ConcurrencyFailureErrorMessage,
                ErrorType.ConcurrencyFailure, ex);
        }
        catch (DbUpdateException ex)
        {
            return Result.Error("Database error while updating user profile.", ErrorType.DatabaseUpdateFailure, ex);
        }
    }

    public async Task<Result<SiteAdmin>> GetSiteAdminAsync(Guid userId)
    {
        try
        {
            var siteAdmin = await _context.SiteAdmins.FirstOrDefaultAsync(sa => sa.UserId == userId);
            return siteAdmin is null
                ? Result<SiteAdmin>.Error($"Site admin not found with ID {userId}", ErrorType.NotFound)
                : Result<SiteAdmin>.Success(siteAdmin);
        }
        catch (OperationCanceledException ex)
        {
            return Result<SiteAdmin>.Error(RepositoryMessageConstants.DatabaseTimeoutErrorMessage,
                ErrorType.DatabaseTimeout, ex);
        }
    }

    public async Task<Result<ImmutableList<SiteAdmin>>> GetSiteAdminsAsync()
    {
        try
        {
            var siteAdmins = await _context.SiteAdmins
                .Include(sa => sa.User)
                    .ThenInclude(u => u.IssuedSiteBans)
                        .ThenInclude(ib => ib.User)
                .ToListAsync();
            
            return Result<ImmutableList<SiteAdmin>>.Success([..siteAdmins]);
        }
        catch (OperationCanceledException ex)
        {
            return Result<ImmutableList<SiteAdmin>>.Error(RepositoryMessageConstants.DatabaseTimeoutErrorMessage,
                ErrorType.DatabaseTimeout, ex);
        }
    }

    public async Task<Result> AddSiteAdminAsync(SiteAdmin siteAdmin)
    {
        try
        {
            _context.SiteAdmins.Add(siteAdmin);
            await _context.SaveChangesAsync();

            return Result.Success();
        }
        catch (OperationCanceledException ex)
        {
            return Result.Error(RepositoryMessageConstants.DatabaseTimeoutErrorMessage,
                ErrorType.DatabaseTimeout, ex);
        }
        catch (DbUpdateConcurrencyException ex)
        {
            return Result.Error(RepositoryMessageConstants.ConcurrencyFailureErrorMessage,
                ErrorType.ConcurrencyFailure, ex);
        }
        catch (DbUpdateException ex)
        {
            return Result.Error("Error while saving the site admin to the database.", ErrorType.DatabaseUpdateFailure,
                ex);
        }
    }

    public async Task<Result> RemoveSiteAdminAsync(SiteAdmin siteAdmin)
    {
        try
        {
            _context.SiteAdmins.Remove(siteAdmin);
            await _context.SaveChangesAsync();

            return Result.Success();
        }
        catch (OperationCanceledException ex)
        {
            return Result.Error(RepositoryMessageConstants.DatabaseTimeoutErrorMessage,
                ErrorType.DatabaseTimeout, ex);
        }
        catch (DbUpdateConcurrencyException ex)
        {
            return Result.Error(RepositoryMessageConstants.ConcurrencyFailureErrorMessage,
                ErrorType.ConcurrencyFailure, ex);
        }
        catch (DbUpdateException ex)
        {
            return Result.Error("Error while saving site admin removal to the database.",
                ErrorType.DatabaseUpdateFailure, ex);
        }
    }

    public async Task<Result<User>> GetCurrentUserSummaryAsync(Guid userId)
    {
        try
        {
            var user = await _context.Users
                .Include(u => u.SiteAdmin)
                .Include(u => u.SiteOwner)
                .Include(u => u.SubThreadSubscriptions)
                    .ThenInclude(s => s.SubThread)
                .Include(u => u.SubThreadOwnerRoles)
                .Include(u => u.SubThreadModeratorRoles)
                .FirstOrDefaultAsync(u => u.Id == userId);

            return user is null
                ? Result<User>.Error($"User not found with ID {userId}", ErrorType.UserNotFound)
                : Result<User>.Success(user);
        }
        catch (OperationCanceledException ex)
        {
            return Result<User>.Error(RepositoryMessageConstants.DatabaseTimeoutErrorMessage,
                ErrorType.DatabaseTimeout, ex);
        }
    }
}