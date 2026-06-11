using Threddit.Application.DTOs.Requests.Comments;
using Threddit.Application.DTOs.Responses.Comments;
using Threddit.Domain.Common;

namespace Threddit.Application.Interfaces.Driving;

public interface IVoteCommentUseCase
{
    /// <summary>Casts or toggles a vote on a comment for the requesting user.</summary>
    /// <remarks>
    /// Possible error types:
    /// <list type="bullet">
    ///     <item><see cref="ErrorType.CommentNotFound"/></item>
    ///     <item><see cref="ErrorType.UserNotFound"/></item>
    ///     <item><see cref="ErrorType.DatabaseTimeout"/></item>
    ///     <item><see cref="ErrorType.ConcurrencyFailure"/></item>
    ///     <item><see cref="ErrorType.DatabaseUpdateFailure"/></item>
    /// </list>
    /// </remarks>
    Task<VoteCommentResponse> ExecuteAsync(VoteCommentRequest request);
}