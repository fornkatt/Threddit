using Microsoft.Extensions.Logging;
using Threddit.Application.DTOs.Common;
using Threddit.Application.DTOs.Requests.Posts;
using Threddit.Application.DTOs.Responses.Posts;
using Threddit.Application.Interfaces.Driven;
using Threddit.Application.Interfaces.Driving;
using Threddit.Application.Mappers;
using Threddit.Domain.Common;

namespace Threddit.Application.UseCases.Posts;

public sealed partial class GetPostByIdWithCommentsUseCase : IGetPostByIdWithCommentsUseCase
{
    private readonly IPostRepository _postRepository;
    private readonly ILogger<GetPostByIdWithCommentsUseCase> _logger;

    public GetPostByIdWithCommentsUseCase(
        IPostRepository postRepository,
        ILogger<GetPostByIdWithCommentsUseCase> logger
    )
    {
        _postRepository = postRepository;
        _logger = logger;
    }

    public async Task<GetPostWithCommentsResponse> ExecuteAsync(GetPostByIdWithCommentsRequest request)
    {
        var fetchResult = await _postRepository.GetByIdWithCommentsAsync(request.PostId);
        if (!fetchResult.IsSuccess)
        {
            LogFetchFailure(fetchResult.Exception, request.PostId, fetchResult.ErrorMessage);
            var message = ResolveErrorMessage(fetchResult.ErrorType);
            return new GetPostWithCommentsResponse(false, Message: message, ErrorType: fetchResult.ErrorType);
        }

        var post = fetchResult.Value!;

        var dto = new PostWithCommentsDto(
            new PostDto(
                post.Id,
                post.SubThreadId,
                post.PostedById,
                post.PostedBy?.UserName,
                post.PostedBy?.ProfilePicture,
                post.Title ?? string.Empty,
                post.Content,
                post.ImageUrl,
                post.Slug,
                post.Score,
                post.CommentCount,
                post.PostedAt,
                post.DeletedAt
            ),
            CommentMapper.MapComments(post.Comments)
        );

        return new GetPostWithCommentsResponse(true, dto);
    }

    private static string ResolveErrorMessage(ErrorType errorType) => errorType switch
    {
        ErrorType.PostNotFound => "Post not found",
        ErrorType.DatabaseTimeout => "The request timed out or was cancelled.",
        _ => "An unknown error occurred. Please try again later."
    };
}