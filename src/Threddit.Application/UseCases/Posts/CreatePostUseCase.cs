using Microsoft.Extensions.Logging;
using Threddit.Application.DTOs.Common;
using Threddit.Application.DTOs.Requests.Posts;
using Threddit.Application.DTOs.Responses.Posts;
using Threddit.Application.Interfaces.Driven;
using Threddit.Application.Interfaces.Driving;
using Threddit.Domain.Common;
using Threddit.Domain.Entities;

namespace Threddit.Application.UseCases.Posts;

public sealed partial class CreatePostUseCase : ICreatePostUseCase
{
    private readonly IPostRepository _postRepository;
    private readonly ISubThreadRepository _subThreadRepository;
    private readonly IUserRepository _userRepository;
    private readonly ILogger<CreatePostUseCase> _logger;

    public CreatePostUseCase(
        IPostRepository postRepository,
        ISubThreadRepository subThreadRepository,
        IUserRepository userRepository,
        ILogger<CreatePostUseCase> logger
    )
    {
        _postRepository = postRepository;
        _subThreadRepository = subThreadRepository;
        _userRepository = userRepository;
        _logger = logger;
    }

    public async Task<CreatePostResponse> ExecuteAsync(CreatePostRequest request)
    {
        var userResult = await _userRepository.GetByIdWithRolesAsync(request.RequestingUserId);
        if (!userResult.IsSuccess)
        {
            LogUserNotFound(userResult.Exception, request.RequestingUserId, userResult.ErrorMessage);
            var message = ResolveErrorMessage(userResult.ErrorType);
            return new CreatePostResponse(false, Message: message, ErrorType: userResult.ErrorType);
        }

        var subThreadResult = await _subThreadRepository.GetByNameAsync(request.SubThreadName);
        if (!subThreadResult.IsSuccess)
        {
            LogSubThreadNotFound(subThreadResult.Exception, request.SubThreadName, subThreadResult.ErrorMessage);
            var message = ResolveErrorMessage(subThreadResult.ErrorType);
            return new CreatePostResponse(false, Message: message, ErrorType: subThreadResult.ErrorType);
        }

        if (userResult.Value!.ReceivedSubThreadBans
            .Any(bs => bs.SubThreadId == subThreadResult.Value!.Id && !bs.IsExpired))
        {
            LogUserSubThreadBanned(userResult.Value.Id, subThreadResult.Value!.Name);
            return new CreatePostResponse(false, Message: "User is banned from this SubThread.",
                ErrorType: ErrorType.SubThreadBanned);
        }

        var createResult = Post.Create(
            userResult.Value!,
            subThreadResult.Value!,
            request.Title,
            request.Content,
            request.ImageUrl
        );

        if (!createResult.IsSuccess)
            return new CreatePostResponse(false, Message: ResolveErrorMessage(createResult.ErrorType),
                ErrorType: createResult.ErrorType);

        var saveResult = await _postRepository.CreateAsync(createResult.Value!);
        if (!saveResult.IsSuccess)
        {
            LogCreationFailure(saveResult.Exception, request.Title, saveResult.ErrorMessage);
            var message = ResolveErrorMessage(saveResult.ErrorType);
            return new CreatePostResponse(false, Message: message, ErrorType: saveResult.ErrorType);
        }

        var post = saveResult.Value!;

        LogPostCreated(post.Id, request.Title, post.PostedBy!.UserName!, request.RequestingUserId);

        return new CreatePostResponse(true, new PostDto(
            post.Id,
            post.SubThreadId,
            post.PostedById,
            post.PostedBy?.UserName,
            post.PostedBy?.ProfilePicture,
            post.Title!,
            post.Content,
            post.ImageUrl,
            post.Slug,
            post.Score,
            post.CommentCount,
            post.PostedAt,
            null
        ));
    }

    private static string ResolveErrorMessage(ErrorType errorType) => errorType switch
    {
        ErrorType.TitleEmpty => "Post title cannot be empty.",
        ErrorType.TitleTooLong => $"Post title cannot exceed {Post.Limits.MaxTitleLength} characters.",
        ErrorType.ContentEmpty => "Post content cannot be empty.",
        ErrorType.ImageUrlTooLong => $"Image URL cannot exceed {Post.Limits.MaxImageUrlLength} characters.",
        ErrorType.SubThreadNotFound => "SubThread not found.",
        ErrorType.UserNotFound => "User not found.",
        ErrorType.ConcurrencyFailure => "Post creation failed due to internal server error. Please refresh and try again.",
        ErrorType.DatabaseTimeout => "The request timed out. Please try again later.",
        _ => "An unexpected error occurred trying to create post. Please try again later."
    };
}