using System.Collections.Immutable;
using Microsoft.EntityFrameworkCore;
using Threddit.Application.DTOs.Common;
using Threddit.Application.DTOs.Requests.SubThreads;
using Threddit.Application.Interfaces.Driven;
using Threddit.Domain.Common;
using Threddit.Domain.Entities;
using Threddit.Infrastructure.Persistence.Repositories.Common;

namespace Threddit.Infrastructure.Persistence.Repositories;

public class SubThreadRepository : ISubThreadRepository
{
    private readonly ThredditDbContext _context;

    public SubThreadRepository(
        ThredditDbContext context
    )
    {
        _context = context;
    }

    public async Task<Result<PagedResult<SubThread>>> SearchAsync(string query, int page, int pageSize)
    {
        try
        {
            var escapedQuery = query
                .Replace("\\", @"\\")
                .Replace("%", "\\%")
                .Replace("_", "\\_");

            var baseQuery = _context.SubThreads
                .Where(st => EF.Functions.Like(st.Name, $"%{escapedQuery}%", "\\"))
                .OrderBy(st => st.Name.StartsWith(query) ? 0 : 1)
                .ThenBy(st => st.Name);

            var totalCount = await baseQuery.CountAsync();

            var items = await baseQuery
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return Result<PagedResult<SubThread>>.Success(new PagedResult<SubThread>(items, page, pageSize,
                totalCount));
        }
        catch (OperationCanceledException ex)
        {
            return Result<PagedResult<SubThread>>.Error(RepositoryMessageConstants.DatabaseTimeoutErrorMessage,
                ErrorType.DatabaseTimeout, ex);
        }
    }

    public async Task<Result<PagedResult<SubThread>>> GetTopAsync(int page, int pageSize)
    {
        try
        {
            var baseQuery = _context.SubThreads
                .OrderByDescending(st => st.SubscriberCount);

            var totalCount = await baseQuery.CountAsync();

            var items = await baseQuery
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return Result<PagedResult<SubThread>>.Success(new PagedResult<SubThread>(items, page, pageSize,
                totalCount));
        }
        catch (OperationCanceledException ex)
        {
            return Result<PagedResult<SubThread>>.Error(RepositoryMessageConstants.DatabaseTimeoutErrorMessage,
                ErrorType.DatabaseTimeout, ex);
        }
    }

    public async Task<Result<SubThread>> GetByNameAsync(string name)
    {
        try
        {
            var subThread = await _context.SubThreads
                .Include(st => st.CreatedBy)
                .Include(st => st.SubThreadOwner)
                .Include(st => st.SubThreadRules)
                .FirstOrDefaultAsync(s => s.Name == name);

            return subThread is null
                ? Result<SubThread>.Error($"SubThread not found with name {name}", ErrorType.SubThreadNotFound)
                : Result<SubThread>.Success(subThread);
        }
        catch (OperationCanceledException ex)
        {
            return Result<SubThread>.Error(RepositoryMessageConstants.DatabaseTimeoutErrorMessage,
                ErrorType.DatabaseTimeout, ex);
        }
    }

    public async Task<Result<PagedResult<Post>>> GetPostsAsync(Guid subThreadId, int page, int pageSize,
        PostSortOrder sortOrder)
    {
        try
        {
            var baseQuery = _context.Posts
                .Where(p => p.SubThreadId == subThreadId)
                .Include(p => p.PostedBy);

            IQueryable<Post> sorted = sortOrder switch
            {
                PostSortOrder.Top => baseQuery.OrderByDescending(p => p.Score),
                PostSortOrder.Hot => baseQuery.OrderByDescending(p => p.Score)
                    .ThenByDescending(p => p.PostedAt),
                _ => baseQuery.OrderByDescending(p => p.PostedAt)
            };

            var totalCount = await sorted.CountAsync();

            var items = await sorted
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return Result<PagedResult<Post>>.Success(new PagedResult<Post>(items, page, pageSize, totalCount));
        }
        catch (OperationCanceledException ex)
        {
            return Result<PagedResult<Post>>.Error(RepositoryMessageConstants.DatabaseTimeoutErrorMessage,
                ErrorType.DatabaseTimeout, ex);
        }
    }

    public async Task<Result<SubThread>> GetByIdAsync(Guid subThreadId)
    {
        try
        {
            var subThread = await _context.SubThreads
                .Include(st => st.SubThreadOwner)
                .Include(st => st.CreatedBy)
                .Include(st => st.SubThreadRules)
                .FirstOrDefaultAsync(st => st.Id == subThreadId);

            return subThread is null
                ? Result<SubThread>.Error($"SubThread not found with ID {subThreadId}", ErrorType.SubThreadNotFound)
                : Result<SubThread>.Success(subThread);
        }
        catch (OperationCanceledException ex)
        {
            return Result<SubThread>.Error(RepositoryMessageConstants.DatabaseTimeoutErrorMessage,
                ErrorType.DatabaseTimeout, ex);
        }
    }

    public async Task<Result<SubThread>> CreateAsync(SubThread subThread)
    {
        try
        {
            _context.SubThreads.Add(subThread);
            await _context.SaveChangesAsync();

            return Result<SubThread>.Success(subThread);
        }
        catch (OperationCanceledException ex)
        {
            return Result<SubThread>.Error(RepositoryMessageConstants.DatabaseTimeoutErrorMessage,
                ErrorType.DatabaseTimeout, ex);
        }
        catch (DbUpdateConcurrencyException ex)
        {
            return Result<SubThread>.Error(RepositoryMessageConstants.ConcurrencyFailureErrorMessage,
                ErrorType.ConcurrencyFailure,
                ex);
        }
        catch (DbUpdateException ex)
        {
            return Result<SubThread>.Error("Database update error while creating SubThread.",
                ErrorType.DatabaseUpdateFailure, ex);
        }
    }

    public async Task<Result> DeleteAsync(SubThread subThread)
    {
        try
        {
            _context.SubThreads.Remove(subThread);
            await _context.SaveChangesAsync();
            return Result.Success();
        }
        catch (OperationCanceledException ex)
        {
            return Result.Error(RepositoryMessageConstants.DatabaseTimeoutErrorMessage, ErrorType.DatabaseTimeout, ex);
        }
        catch (DbUpdateConcurrencyException ex)
        {
            return Result.Error(RepositoryMessageConstants.ConcurrencyFailureErrorMessage, ErrorType.ConcurrencyFailure,
                ex);
        }
        catch (DbUpdateException ex)
        {
            return Result.Error("Database error while deleting SubThread.", ErrorType.DatabaseUpdateFailure, ex);
        }
    }

    public async Task<Result<SubThreadSubscription?>> GetSubscriptionAsync(Guid subThreadId, Guid userId)
    {
        try
        {
            return Result<SubThreadSubscription?>.Success(await _context.SubThreadSubscriptions
                .FirstOrDefaultAsync(st => st.SubThreadId == subThreadId && st.UserId == userId));
        }
        catch (OperationCanceledException ex)
        {
            return Result<SubThreadSubscription?>.Error(RepositoryMessageConstants.DatabaseTimeoutErrorMessage,
                ErrorType.DatabaseTimeout, ex);
        }
    }

    public async Task<Result> SubscribeAsync(SubThreadSubscription subscription, SubThread subThread)
    {
        await using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            _context.SubThreadSubscriptions.Add(subscription);
            _context.SubThreads.Update(subThread);

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            return Result.Success();
        }
        catch (OperationCanceledException ex)
        {
            await transaction.RollbackAsync();
            return Result.Error(RepositoryMessageConstants.DatabaseTimeoutErrorMessage, ErrorType.DatabaseTimeout, ex);
        }
        catch (DbUpdateConcurrencyException ex)
        {
            await transaction.RollbackAsync();
            return Result.Error(RepositoryMessageConstants.ConcurrencyFailureErrorMessage,
                ErrorType.ConcurrencyFailure, ex);
        }
        catch (DbUpdateException ex)
        {
            await transaction.RollbackAsync();
            return Result.Error("Database error while saving subscription.", ErrorType.DatabaseUpdateFailure, ex);
        }
    }

    public async Task<Result> UnsubscribeAsync(SubThreadSubscription subscription, SubThread subThread)
    {
        await using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            _context.SubThreadSubscriptions.Remove(subscription);
            _context.SubThreads.Update(subThread);

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            return Result.Success();
        }
        catch (OperationCanceledException ex)
        {
            await transaction.RollbackAsync();
            return Result.Error(RepositoryMessageConstants.DatabaseTimeoutErrorMessage, ErrorType.DatabaseTimeout, ex);
        }
        catch (DbUpdateConcurrencyException ex)
        {
            await transaction.RollbackAsync();
            return Result.Error(RepositoryMessageConstants.ConcurrencyFailureErrorMessage,
                ErrorType.ConcurrencyFailure, ex);
        }
        catch (DbUpdateException ex)
        {
            await transaction.RollbackAsync();
            return Result.Error("Database error while removing subscription.", ErrorType.DatabaseUpdateFailure, ex);
        }
    }

    public async Task<Result> UpdateAsync(SubThread subThread)
    {
        try
        {
            _context.SubThreads.Update(subThread);
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
            return Result.Error("Database error while updating SubThread.", ErrorType.DatabaseUpdateFailure, ex);
        }
    }

    public async Task<Result<SubThreadRule>> GetRuleByIdAsync(Guid ruleId)
    {
        try
        {
            var rule = await _context.SubThreadRules
                .FirstOrDefaultAsync(r => r.Id == ruleId);

            return rule is null
                ? Result<SubThreadRule>.Error($"Rule not found with id {ruleId}", ErrorType.SubThreadRuleNotFound)
                : Result<SubThreadRule>.Success(rule);
        }
        catch (OperationCanceledException ex)
        {
            return Result<SubThreadRule>.Error(RepositoryMessageConstants.DatabaseTimeoutErrorMessage,
                ErrorType.DatabaseTimeout, ex);
        }
    }

    public async Task<Result<SubThreadRule>> AddRuleAsync(SubThreadRule rule)
    {
        try
        {
            _context.SubThreadRules.Add(rule);
            await _context.SaveChangesAsync();

            return Result<SubThreadRule>.Success(rule);
        }
        catch (OperationCanceledException ex)
        {
            return Result<SubThreadRule>.Error(RepositoryMessageConstants.DatabaseTimeoutErrorMessage,
                ErrorType.DatabaseTimeout, ex);
        }
        catch (DbUpdateConcurrencyException ex)
        {
            return Result<SubThreadRule>.Error(RepositoryMessageConstants.ConcurrencyFailureErrorMessage,
                ErrorType.ConcurrencyFailure, ex);
        }
        catch (DbUpdateException ex)
        {
            return Result<SubThreadRule>.Error("Database error while adding rule.", ErrorType.DatabaseUpdateFailure,
                ex);
        }
    }

    public async Task<Result> UpdateRuleAsync(SubThreadRule rule)
    {
        try
        {
            _context.SubThreadRules.Update(rule);
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
            return Result.Error("Database error while updating rule.", ErrorType.DatabaseUpdateFailure, ex);
        }
    }

    public async Task<Result> DeleteRuleAsync(SubThreadRule rule)
    {
        try
        {
            _context.SubThreadRules.Remove(rule);
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
            return Result.Error("Database error while deleting rule.", ErrorType.DatabaseUpdateFailure, ex);
        }
    }

    public async Task<Result<SubThreadModerator?>> GetModeratorAsync(Guid subThreadId, Guid userId)
    {
        try
        {
            return Result<SubThreadModerator?>.Success(await _context.SubThreadModerators
                .FirstOrDefaultAsync(sm => sm.SubThreadId == subThreadId && sm.UserId == userId));
        }
        catch (OperationCanceledException ex)
        {
            return Result<SubThreadModerator?>.Error(RepositoryMessageConstants.DatabaseTimeoutErrorMessage,
                ErrorType.DatabaseTimeout, ex);
        }
    }

    public async Task<Result> AddModeratorAsync(SubThreadModerator moderator)
    {
        try
        {
            _context.SubThreadModerators.Add(moderator);
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
            return Result.Error("Database error while adding moderator.", ErrorType.DatabaseUpdateFailure, ex);
        }
    }

    public async Task<Result> RemoveModeratorAsync(SubThreadModerator moderator)
    {
        try
        {
            _context.SubThreadModerators.Remove(moderator);
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
            return Result.Error("Database error while removing moderator.", ErrorType.DatabaseUpdateFailure, ex);
        }
    }

    public async Task<Result<ImmutableList<SubThreadModerator>>> GetModeratorsAsync(string subThreadName)
    {
        try
        {
            var subThread = await _context.SubThreads
                .FirstOrDefaultAsync(st => st.Name == subThreadName);
            
            if (subThread is null)
                return Result<ImmutableList<SubThreadModerator>>.Error($"SubThread not found with name {subThreadName}",
                    ErrorType.SubThreadNotFound);
            
            var moderators = await _context.SubThreadModerators
                .Include(m => m.User)
                .Where(m => m.SubThreadId == subThread.Id)
                .ToListAsync();

            return Result<ImmutableList<SubThreadModerator>>.Success([..moderators]);
        }
        catch (OperationCanceledException ex)
        {
            return Result<ImmutableList<SubThreadModerator>>.Error(
                RepositoryMessageConstants.DatabaseTimeoutErrorMessage, ErrorType.DatabaseTimeout, ex);
        }
    }
}