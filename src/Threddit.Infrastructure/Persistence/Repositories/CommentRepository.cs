using Microsoft.EntityFrameworkCore;
using Threddit.Application.Interfaces.Driven;
using Threddit.Domain.Common;
using Threddit.Domain.Entities;
using Threddit.Infrastructure.Persistence.Repositories.Common;

namespace Threddit.Infrastructure.Persistence.Repositories;

public sealed class CommentRepository : ICommentRepository
{
    private readonly ThredditDbContext _context;

    public CommentRepository(
        ThredditDbContext context
    )
    {
        _context = context;
    }

    public async Task<Result<IReadOnlyList<Comment>>> GetRepliesAsync(Guid parentCommentId)
    {
        try
        {
            var replies = await _context.Comments
                .Where(c => c.ParentCommentId == parentCommentId)
                .Include(c => c.CommentedBy)
                .Include(c => c.Replies)
                .ThenInclude(c => c.CommentedBy)
                .ToListAsync();

            return Result<IReadOnlyList<Comment>>.Success(replies);
        }
        catch (OperationCanceledException ex)
        {
            return Result<IReadOnlyList<Comment>>.Error(RepositoryMessageConstants.DatabaseTimeoutErrorMessage,
                ErrorType.DatabaseTimeout, ex);
        }
    }

    public async Task<Result<Comment>> GetByIdAsync(Guid commentId)
    {
        try
        {
            var comment = await _context.Comments
                .FirstOrDefaultAsync(c => c.Id == commentId);

            return comment is null
                ? Result<Comment>.Error($"Comment not found with ID {commentId}", ErrorType.CommentNotFound)
                : Result<Comment>.Success(comment);
        }
        catch (OperationCanceledException ex)
        {
            return Result<Comment>.Error(RepositoryMessageConstants.DatabaseTimeoutErrorMessage,
                ErrorType.DatabaseTimeout, ex);
        }
    }

    public async Task<Result<Comment>> CreateWithCounterUpdatesAsync(Comment comment, Post post,
        Comment? parentComment = null)
    {
        await using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            _context.Comments.Add(comment);
            _context.Posts.Update(post);

            if (parentComment is not null)
                _context.Comments.Update(parentComment);

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            return Result<Comment>.Success(comment);
        }
        catch (OperationCanceledException ex)
        {
            await transaction.RollbackAsync();
            return Result<Comment>.Error(RepositoryMessageConstants.DatabaseTimeoutErrorMessage,
                ErrorType.DatabaseTimeout, ex);
        }
        catch (DbUpdateConcurrencyException ex)
        {
            await transaction.RollbackAsync();
            return Result<Comment>.Error(RepositoryMessageConstants.ConcurrencyFailureErrorMessage,
                ErrorType.ConcurrencyFailure, ex);
        }
        catch (DbUpdateException ex)
        {
            await transaction.RollbackAsync();
            return Result<Comment>.Error($"Database error creating comment while saving to database",
                ErrorType.DatabaseUpdateFailure, ex);
        }
    }

    public async Task<Result<Comment>> UpdateAsync(Comment comment)
    {
        try
        {
            _context.Comments.Update(comment);
            await _context.SaveChangesAsync();

            return Result<Comment>.Success(comment);
        }
        catch (OperationCanceledException ex)
        {
            return Result<Comment>.Error(RepositoryMessageConstants.DatabaseTimeoutErrorMessage,
                ErrorType.DatabaseTimeout, ex);
        }
        catch (DbUpdateConcurrencyException ex)
        {
            return Result<Comment>.Error(RepositoryMessageConstants.ConcurrencyFailureErrorMessage,
                ErrorType.ConcurrencyFailure, ex);
        }
        catch (DbUpdateException ex)
        {
            return Result<Comment>.Error($"Database error updating comment while saving to database",
                ErrorType.DatabaseUpdateFailure, ex);
        }
    }

    public async Task<Result<CommentVote?>> GetVoteAsync(Guid commentId, Guid userId)
    {
        try
        {
            return Result<CommentVote?>.Success(await _context.CommentVotes
                .FirstOrDefaultAsync(cv => cv.CommentId == commentId && cv.UserId == userId));
        }
        catch (OperationCanceledException ex)
        {
            return Result<CommentVote?>.Error(RepositoryMessageConstants.DatabaseTimeoutErrorMessage,
                ErrorType.DatabaseTimeout, ex);
        }
    }

    public async Task<Result<int>> ProcessVoteAsync(Guid commentId, Guid userId, bool isUpvote)
    {
        await using var transaction = await _context.Database.BeginTransactionAsync(
            System.Data.IsolationLevel.Serializable);
        try
        {
            var existingVote = await _context.CommentVotes
                .FromSqlRaw(
                    "SELECT * FROM [threddit].[Comment_CommentVotes] WITH (UPDLOCK, ROWLOCK) WHERE CommentId = {0} AND UserId = {1}",
                    commentId, userId)
                .FirstOrDefaultAsync();

            var comment = await _context.Comments.FirstOrDefaultAsync(c => c.Id == commentId && !c.IsDeleted);
            if (comment is null)
                return Result<int>.Error("Comment not found.", ErrorType.CommentNotFound);

            var author = comment.CommentedById.HasValue
                ? await _context.Users.FirstOrDefaultAsync(u => u.Id == comment.CommentedById.Value)
                : null;

            int scoreDelta;

            if (existingVote is null)
            {
                var user = await _context.Users.FirstAsync(u => u.Id == userId);
                var newVote = CommentVote.Create(comment, user, isUpvote);
                _context.CommentVotes.Add(newVote);
                scoreDelta = isUpvote ? 1 : -1;
            }
            else
            {
                var wasUpvote = existingVote.IsUpvote;
                var wasDownvote = existingVote.IsDownvote;

                if (isUpvote) existingVote.Upvote();
                else existingVote.Downvote();

                if (existingVote is { IsUpvote: false, IsDownvote: false })
                {
                    _context.CommentVotes.Remove(existingVote);
                    scoreDelta = wasUpvote ? -1 : 1;
                }
                else
                {
                    scoreDelta = (wasUpvote, wasDownvote, existingVote.IsUpvote, existingVote.IsDownvote) switch
                    {
                        (true, false, false, true) => -2,
                        (false, true, true, false) => 2,
                        _ => 0
                    };
                    _context.CommentVotes.Update(existingVote);
                }
            }

            comment.ApplyVoteDelta(scoreDelta);
            author?.ApplyCommentScoreDelta(scoreDelta);

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            return Result<int>.Success(comment.Score);
        }
        catch (OperationCanceledException ex)
        {
            await transaction.RollbackAsync();
            return Result<int>.Error(RepositoryMessageConstants.DatabaseTimeoutErrorMessage, ErrorType.DatabaseTimeout,
                ex);
        }
        catch (DbUpdateException ex)
        {
            await transaction.RollbackAsync();
            return Result<int>.Error("Database constraint or update failure", ErrorType.DatabaseUpdateFailure, ex);
        }
    }
}