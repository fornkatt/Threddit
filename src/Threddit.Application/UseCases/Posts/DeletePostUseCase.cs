using Microsoft.Extensions.Logging;
using Threddit.Application.DTOs.Requests.Posts;
using Threddit.Application.DTOs.Responses.Posts;
using Threddit.Application.Interfaces.Driven;
using Threddit.Application.Interfaces.Driving;
using Threddit.Domain.Common;

namespace Threddit.Application.UseCases.Posts;

public sealed partial class DeletePostUseCase : IDeletePostUseCase
{
    private readonly IPostRepository _postRepository;
    private readonly ILogger<DeletePostUseCase> _logger;

    public DeletePostUseCase(
        IPostRepository postRepository,
        ILogger<DeletePostUseCase> logger
    )
    {
        _postRepository = postRepository;
        _logger = logger;
    }

    public async Task<DeletePostResponse> ExecuteAsync(DeletePostRequest request)
    {
        var fetchResult = await _postRepository.GetByIdAsync(request.PostId);
        if (!fetchResult.IsSuccess)
        {
            LogFetchFailure(request.PostId, fetchResult.ErrorMessage);
            var message = ResolveErrorMessage(fetchResult.ErrorType);
            return new DeletePostResponse(false, message, fetchResult.ErrorType);
        }

        var post = fetchResult.Value!;

        var isSitePrivileged = request.IsSiteAdmin || request.IsSiteOwner;
        var isModerator = request.ModeratedSubThreadIds.Contains(post.SubThreadId);
        var isAuthor = post.PostedById == request.RequestingUserId;

        if (!isSitePrivileged && !isModerator && !isAuthor)
        {
            LogUnauthorizedDeletionAttempt(request.RequestingUserId, request.PostId);
            var message = ResolveErrorMessage(ErrorType.Forbidden);
            return new DeletePostResponse(false, message, ErrorType.Forbidden);
        }

        if (!isAuthor && (isSitePrivileged || isModerator) && string.IsNullOrWhiteSpace(request.Reason))
        {
            LogDeletionFailure(request.PostId, "Reason is required for site admins and moderators.");
            var message = ResolveErrorMessage(ErrorType.DeleteReasonRequired);
            return new DeletePostResponse(false, message, ErrorType.DeleteReasonRequired);
        }

        var deleteResult = post.SoftDelete(request.Reason);
        if (!deleteResult.IsSuccess)
        {
            LogDeletionFailure(request.PostId, deleteResult.ErrorMessage);
            var message = ResolveErrorMessage(deleteResult.ErrorType);
            return new DeletePostResponse(false, message, deleteResult.ErrorType);
        }

        var transactionResult = await _postRepository.DeletePostAndRelatedDataAsync(post);
        if (!transactionResult.IsSuccess)
        {
            LogDatabaseUpdateFailure(transactionResult.Exception, transactionResult.ErrorMessage, post.Id);
            var message = ResolveErrorMessage(transactionResult.ErrorType);
            return new DeletePostResponse(false, message, transactionResult.ErrorType);
        }

        LogDeletionSuccess(post.Id);
        return new DeletePostResponse(true, "Post deletion successful.");
    }

    private static string ResolveErrorMessage(ErrorType errorType)
    {
        return errorType switch
        {
            ErrorType.PostNotFound => "Post not found.",
            ErrorType.AlreadyDeleted => "Post is already deleted.",
            ErrorType.Forbidden => "You are not authorized to delete this post.",
            ErrorType.DeleteReasonRequired => "Delete reason is required.",
            ErrorType.ConcurrencyFailure => "The post may have been modified by another user. Please refresh and try again.",
            ErrorType.DatabaseTimeout => "The request timed out or was cancelled.",
            _ => "An unknown error occurred. Please try again later."
        };
    }
}