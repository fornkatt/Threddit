using Threddit.Application.DTOs.Requests.Comments;
using Threddit.Application.DTOs.Responses.Comments;
using Threddit.Domain.Common;

namespace Threddit.Application.Interfaces.Driving;

public interface IEditCommentUseCase
{
    /// <summary>Edits the content of an existing comment.</summary>
    /// <remarks>
    /// Only the original commenter may edit their own comment.
    /// <br/><br/>
    /// Possible error types:
    /// <list type="bullet">
    ///     <item><see cref="ErrorType.CommentNotFound"/></item>
    ///     <item><see cref="ErrorType.Forbidden"/></item>
    ///     <item><see cref="ErrorType.AlreadyDeleted"/></item>
    ///     <item><see cref="ErrorType.ContentEmpty"/></item>
    ///     <item><see cref="ErrorType.ContentTooLong"/></item>
    ///     <item><see cref="ErrorType.ConcurrencyFailure"/></item>
    ///     <item><see cref="ErrorType.DatabaseUpdateFailure"/></item>
    ///     <item><see cref="ErrorType.DatabaseTimeout"/></item>
    /// </list>
    /// </remarks>
    Task<EditCommentResponse> ExecuteAsync(EditCommentRequest request);
}