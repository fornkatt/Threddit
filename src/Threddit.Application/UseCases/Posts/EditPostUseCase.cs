using Microsoft.Extensions.Logging;
using Threddit.Application.DTOs.Requests.Posts;
using Threddit.Application.DTOs.Responses.Posts;
using Threddit.Application.Interfaces.Driven;
using Threddit.Application.Interfaces.Driving;
using Threddit.Domain.Common;
using Threddit.Domain.Entities;

namespace Threddit.Application.UseCases.Posts;

public sealed partial class EditPostUseCase : IEditPostUseCase
{
    private readonly IPostRepository _postRepository;
    private readonly ILogger<EditPostUseCase> _logger;

    public EditPostUseCase(
        IPostRepository postRepository,
        ILogger<EditPostUseCase> logger
    )
    {
        _postRepository = postRepository;
        _logger = logger;
    }

    public async Task<EditPostResponse> ExecuteAsync(EditPostRequest request)
    {
        var fetchResult = await _postRepository.GetByIdAsync(request.PostId);
        if (!fetchResult.IsSuccess)
        {
            LogPostFetchFailure(fetchResult.Exception, request.PostId, fetchResult.ErrorMessage);
            var message = ResolveErrorMessage(fetchResult.ErrorType);
            return new EditPostResponse(false, message, fetchResult.ErrorType);
        }
        var post = fetchResult.Value!;

        if (post.PostedById != request.RequestingUserId)
        {
            LogUnauthorizedEditAttempt(request.RequestingUserId, post.Id);
            var message = ResolveErrorMessage(ErrorType.Forbidden);
            return new EditPostResponse(false, message, ErrorType.Forbidden);
        }
        if (post.IsDeleted)
        {
            LogEditDeletedPost(request.PostId);
            var message = ResolveErrorMessage(ErrorType.AlreadyDeleted);
            return new EditPostResponse(false, message, ErrorType.AlreadyDeleted);
        }

        var editResult = post.Edit(request.NewContent, request.NewImageUrl);
        if (!editResult.IsSuccess)
        {
            LogValidationFailure(post.Id, editResult.ErrorMessage);
            var message = ResolveErrorMessage(editResult.ErrorType);
            return new EditPostResponse(false, message, editResult.ErrorType);
        }
        
        var updateResult = await _postRepository.UpdateAsync(post);
        if (!updateResult.IsSuccess)
        {
            LogUpdateFailure(updateResult.Exception, post.Id, updateResult.ErrorMessage);
            var message = ResolveErrorMessage(updateResult.ErrorType);
            return new EditPostResponse(false, message, updateResult.ErrorType);
        }
        
        LogEditSuccess(post.Id);
        return new EditPostResponse(true, "Post edited successfully.");
    }
    
    private static string ResolveErrorMessage(ErrorType errorType) => errorType switch
    {
        ErrorType.PostNotFound => "Post not found.",
        ErrorType.Forbidden => "You are not authorized to edit this post.",
        ErrorType.AlreadyDeleted => "Cannot edit a deleted post.",
        ErrorType.ContentEmpty => "Post content cannot be empty.",
        ErrorType.ContentTooLong => $"Post content cannot exceed {Post.Limits.MaxContentLength} characters.",
        ErrorType.ImageUrlTooLong => $"Image URL cannot exceed {Post.Limits.MaxImageUrlLength} characters.",
        ErrorType.ConcurrencyFailure => "Failed to update post due to internal server error. Please refresh and try again.",
        ErrorType.DatabaseTimeout => "The request timed out. Please try again later.",
        _ => "An unexpected error occurred. Please try again later."
    };
}