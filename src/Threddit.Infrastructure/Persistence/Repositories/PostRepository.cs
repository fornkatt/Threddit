using Microsoft.EntityFrameworkCore;
using Threddit.Application.Interfaces.Driven;
using Threddit.Domain.Common;
using Threddit.Domain.Entities;
using Threddit.Infrastructure.Persistence.Repositories.Common;

namespace Threddit.Infrastructure.Persistence.Repositories;

public sealed class PostRepository : IPostRepository
{
    private readonly ThredditDbContext _context;

    public PostRepository(
        ThredditDbContext context
    )
    {
        _context = context;
    }

    public async Task<Result<Post>> GetByIdAsync(Guid postId)
    {
        try
        {
            var post = await _context.Posts.FirstOrDefaultAsync(p => p.Id == postId);


            return post is null
                ? Result<Post>.Error($"Could not find post with ID: {postId}", ErrorType.PostNotFound)
                : Result<Post>.Success(post);
        }
        catch (OperationCanceledException ex)
        {
            return Result<Post>.Error(RepositoryMessageConstants.DatabaseTimeoutErrorMessage,
                ErrorType.DatabaseTimeout, ex);
        }
    }

    public async Task<Result<Post>> GetByIdWithCommentsAsync(Guid postId)
    {
        try
        {
            var post = await _context.Posts
                .AsNoTracking()
                .Include(p => p.PostedBy)
                .Include(p => p.Comments.Where(c => c.ParentCommentId == null))
                .ThenInclude(c => c.CommentedBy)
                .Include(p => p.Comments.Where(c => c.ParentCommentId == null))
                .ThenInclude(c => c.Replies)
                .ThenInclude(c => c.CommentedBy)
                .FirstOrDefaultAsync(p => p.Id == postId);

            if (post is null)
                return Result<Post>.Error($"Could not find post with ID: {postId}", ErrorType.PostNotFound);

            return Result<Post>.Success(post);
        }
        catch (OperationCanceledException ex)
        {
            return Result<Post>.Error(RepositoryMessageConstants.DatabaseTimeoutErrorMessage,
                ErrorType.DatabaseTimeout, ex);
        }
    }

    public async Task<Result<Post>> CreateAsync(Post post)
    {
        try
        {
            _context.Posts.Add(post);
            await _context.SaveChangesAsync();
            return Result<Post>.Success(post);
        }
        catch (OperationCanceledException ex)
        {
            return Result<Post>.Error(RepositoryMessageConstants.DatabaseTimeoutErrorMessage,
                ErrorType.DatabaseTimeout, ex);
        }
        catch (DbUpdateConcurrencyException ex)
        {
            return Result<Post>.Error(RepositoryMessageConstants.ConcurrencyFailureErrorMessage,
                ErrorType.ConcurrencyFailure, ex);
        }
        catch (DbUpdateException ex)
        {
            return Result<Post>.Error($"Database error creating post while saving to database",
                ErrorType.DatabaseUpdateFailure, ex);
        }
    }

    public async Task<Result> DeletePostAndRelatedDataAsync(Post post)
    {
        try
        {
            await using var transaction = await _context.Database.BeginTransactionAsync();

            await ScrubCommentsByPostIdAsync(post.Id);

            await _context.SavedPosts
                .Where(sp => sp.PostId == post.Id)
                .ExecuteDeleteAsync();

            var updateResult = await UpdateAsync(post);

            if (!updateResult.IsSuccess)
            {
                await transaction.RollbackAsync();
                return updateResult;
            }

            await transaction.CommitAsync();
            return Result.Success();
        }
        catch (OperationCanceledException ex)
        {
            return Result.Error(RepositoryMessageConstants.DatabaseTimeoutErrorMessage, ErrorType.DatabaseTimeout, ex);
        }
    }

    public async Task<Result> UpdateAsync(Post post)
    {
        try
        {
            _context.Posts.Update(post);
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
            return Result.Error("Database constraint or update failure", ErrorType.DatabaseUpdateFailure, ex);
        }
    }

    public async Task<Result<PostVote?>> GetVoteAsync(Guid postId, Guid userId)
    {
        try
        {
            return Result<PostVote?>.Success(await _context.PostVotes
                .FirstOrDefaultAsync(pv => pv.PostId == postId && pv.UserId == userId));
        }
        catch (OperationCanceledException ex)
        {
            return Result<PostVote?>.Error(RepositoryMessageConstants.DatabaseTimeoutErrorMessage,
                ErrorType.DatabaseTimeout, ex);
        }
    }

    public async Task<Result<int>> ProcessVoteAsync(Guid postId, Guid userId, bool isUpvote)
    {
        await using var transaction = await _context.Database.BeginTransactionAsync(
            System.Data.IsolationLevel.Serializable);
        try
        {
            var existingVote = await _context.PostVotes
                .FromSqlRaw(
                    "SELECT * FROM [threddit].[Post_PostVotes] WITH (UPDLOCK, ROWLOCK) WHERE PostId = {0} AND UserId = {1}",
                    postId, userId)
                .FirstOrDefaultAsync();
            
            var post = await _context.Posts.FirstOrDefaultAsync(p => p.Id == postId && !p.IsDeleted);
            if (post is null)
                return Result<int>.Error("Post not found.", ErrorType.PostNotFound);

            var author = post.PostedById.HasValue
                ? await _context.Users.FirstOrDefaultAsync(u => u.Id == post.PostedById.Value)
                : null;

            int scoreDelta;

            if (existingVote is null)
            {
                var user = await _context.Users.FirstAsync(u => u.Id == userId);
                var newVote = PostVote.Create(post, user, isUpvote);
                _context.PostVotes.Add(newVote);
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
                    _context.PostVotes.Remove(existingVote);
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
                    _context.PostVotes.Update(existingVote);
                }
            }
            
            post.ApplyVoteDelta(scoreDelta);
            author?.ApplyPostScoreDelta(scoreDelta);

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            return Result<int>.Success(post.Score);
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

    private async Task ScrubCommentsByPostIdAsync(Guid postId)
    {
        await _context.SavedComments
            .Where(sc => sc.Comment.PostId == postId)
            .ExecuteDeleteAsync();

        await _context.Comments
            .Where(c => c.PostId == postId && !c.IsDeleted)
            .ExecuteUpdateAsync(s => s
                .SetProperty(c => c.IsDeleted, true)
                .SetProperty(c => c.Content, (string?)null)
                .SetProperty(c => c.ImageUrl, (string?)null)
                .SetProperty(c => c.DeletedAt, DateTime.UtcNow));
    }
}