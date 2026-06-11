using Microsoft.Extensions.Logging;
using Threddit.Application.DTOs.Requests.Posts;
using Threddit.Application.DTOs.Responses.Posts;
using Threddit.Application.Interfaces.Driven;
using Threddit.Application.Interfaces.Driving;
using Threddit.Domain.Common;

namespace Threddit.Application.UseCases.Posts;

public sealed partial class VotePostUseCase : IVotePostUseCase
{
    private readonly IPostRepository _postRepository;
    private readonly ILogger<VotePostUseCase> _logger;

    public VotePostUseCase(
        IPostRepository postRepository,
        ILogger<VotePostUseCase> logger
    )
    {
        _postRepository = postRepository;
        _logger = logger;
    }

    public async Task<VotePostResponse> ExecuteAsync(VotePostRequest request)
    {
        var result = await _postRepository.ProcessVoteAsync(
            request.PostId, request.RequestingUserId, request.IsUpvote);

        if (!result.IsSuccess)
        {
            LogSaveFailure(result.Exception, request.PostId, request.RequestingUserId, result.ErrorMessage);
            var message = ResolveErrorMessage(result.ErrorType);
            return new VotePostResponse(false, Message: message, ErrorType: result.ErrorType);
        }

        LogVoteSuccess(request.PostId, request.RequestingUserId);
        return new VotePostResponse(true, result.Value, "Vote recorded successfully.");
    }

    private static string ResolveErrorMessage(ErrorType errorType) => errorType switch
    {
        ErrorType.PostNotFound => "Post not found.",
        ErrorType.UserNotFound => "User not found.",
        ErrorType.DatabaseTimeout => "The request timed out. Please try again later.",
        ErrorType.ConcurrencyFailure => "Failed to save vote. Please try again later.",
        _ => "An unexpected error occurred. Please try again later."
    };
}