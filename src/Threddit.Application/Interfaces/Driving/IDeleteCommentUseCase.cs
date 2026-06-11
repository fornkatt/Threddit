using Threddit.Application.DTOs.Requests.Comments;
using Threddit.Application.DTOs.Responses.Comments;
using Threddit.Domain.Common;

namespace Threddit.Application.Interfaces.Driving;

public interface IDeleteCommentUseCase
{
    /// <summary>Soft-deletes a comment, scrubbing its content.</summary>
    /// <remarks>
    /// Possible error types:
    /// <list type="bullet">
    ///     <item><see cref="ErrorType.CommentNotFound"/></item>
    ///     <item><see cref="ErrorType.AlreadyDeleted"/></item>
    ///     <item><see cref="ErrorType.DeleteReasonTooLong"/></item>
    ///     <item><see cref="ErrorType.DeleteReasonRequired"/></item>
    ///     <item><see cref="ErrorType.Forbidden"/></item>
    ///     <item><see cref="ErrorType.DatabaseTimeout"/></item>
    ///     <item><see cref="ErrorType.ConcurrencyFailure"/></item>
    ///     <item><see cref="ErrorType.DatabaseUpdateFailure"/></item>
    /// </list>
    /// </remarks>
    Task<DeleteCommentResponse> ExecuteAsync(DeleteCommentRequest request);
}