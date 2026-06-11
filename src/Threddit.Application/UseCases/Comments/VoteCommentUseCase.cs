using Microsoft.Extensions.Logging;
using Threddit.Application.DTOs.Requests.Comments;
using Threddit.Application.DTOs.Responses.Comments;
using Threddit.Application.Interfaces.Driven;
using Threddit.Application.Interfaces.Driving;
using Threddit.Domain.Common;
using Threddit.Domain.Entities;

namespace Threddit.Application.UseCases.Comments;

public sealed partial class VoteCommentUseCase : IVoteCommentUseCase
{
    private readonly ICommentRepository _commentRepository;
    private readonly ILogger<VoteCommentUseCase> _logger;

    public VoteCommentUseCase(
        ICommentRepository commentRepository,
        ILogger<VoteCommentUseCase> logger
    )
    {
        _commentRepository = commentRepository;
        _logger = logger;
    }

    public async Task<VoteCommentResponse> ExecuteAsync(VoteCommentRequest request)
    {
        var result = await _commentRepository.ProcessVoteAsync(
            request.CommentId, request.RequestingUserId, request.IsUpvote);

        if (!result.IsSuccess)
        {
            LogSaveFailure(result.Exception, request.CommentId, request.RequestingUserId, result.ErrorMessage);
            var message = ResolveErrorMessage(result.ErrorType);
            return new VoteCommentResponse(false, Message: message, ErrorType: result.ErrorType);
        }

        LogVoteSuccess(request.CommentId, request.RequestingUserId);
        return new VoteCommentResponse(true, result.Value, "Vote recorded successfully.");
    }

    private static string ResolveErrorMessage(ErrorType errorType) => errorType switch
    {
        ErrorType.CommentNotFound => "Comment not found.",
        ErrorType.UserNotFound => "User not found.",
        ErrorType.DatabaseTimeout => "The request timed out. Please try again later.",
        ErrorType.ConcurrencyFailure => "Failed to save vote. Please try again later.",
        _ => "An unexpected error occurred. Please try again later."
    };
}