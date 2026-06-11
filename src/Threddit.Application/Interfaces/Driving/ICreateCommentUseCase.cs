using Threddit.Application.DTOs.Requests.Comments;
using Threddit.Application.DTOs.Responses.Comments;
using Threddit.Domain.Common;

namespace Threddit.Application.Interfaces.Driving;

public interface ICreateCommentUseCase
{
    /// <summary>Requests the creation of a new comment.</summary>
    /// <remarks>
    /// Possible error types:
    /// <list type="bullet">
    ///     <item><see cref="ErrorType.UserNotFound"/></item>
    ///     <item><see cref="ErrorType.PostNotFound"/></item>
    ///     <item><see cref="ErrorType.CommentNotFound"/></item>
    ///     <item><see cref="ErrorType.SubThreadBanned"/></item>
    ///     <item><see cref="ErrorType.ContentEmpty"/></item>
    ///     <item><see cref="ErrorType.ContentTooLong"/></item>
    ///     <item><see cref="ErrorType.DatabaseTimeout"/></item>
    ///     <item><see cref="ErrorType.ConcurrencyFailure"/></item>
    ///     <item><see cref="ErrorType.DatabaseUpdateFailure"/></item>
    /// </list>
    /// </remarks>
    Task<CreateCommentResponse> ExecuteAsync(CreateCommentRequest request);
}