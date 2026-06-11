using Threddit.Application.DTOs.Requests.Posts;
using Threddit.Application.DTOs.Responses.Posts;
using Threddit.Domain.Common;

namespace Threddit.Application.Interfaces.Driving;

public interface IDeletePostUseCase
{
    /// <summary>Requests the deletion of an existing post.</summary>
    /// <remarks>
    /// Possible error types:
    /// <list type="bullet">
    ///     <item><see cref="ErrorType.PostNotFound"/></item>
    ///     <item><see cref="ErrorType.DatabaseTimeout"/></item>
    ///     <item><see cref="ErrorType.Forbidden"/></item>
    ///     <item><see cref="ErrorType.DeleteReasonRequired"/></item>
    ///     <item><see cref="ErrorType.AlreadyDeleted"/></item>
    ///     <item><see cref="ErrorType.DeleteReasonTooLong"/></item>
    ///     <item><see cref="ErrorType.DatabaseUpdateFailure"/></item>
    ///     <item><see cref="ErrorType.ConcurrencyFailure"/></item>
    /// </list>
    /// </remarks>
    Task<DeletePostResponse> ExecuteAsync(DeletePostRequest request);
}