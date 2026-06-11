using Threddit.Application.DTOs.Requests.Comments;
using Threddit.Application.DTOs.Responses.Comments;
using Threddit.Domain.Common;

namespace Threddit.Application.Interfaces.Driving;

public interface IGetCommentRepliesUseCase
{
    /// <summary>Requests a set of replies to a comment.</summary>
    /// <remarks>
    /// Possible error types:
    /// <list type="bullet">
    ///     <item><see cref="ErrorType.DatabaseTimeout"/></item>
    /// </list>
    /// </remarks>
    Task<GetCommentRepliesResponse> ExecuteAsync(GetCommentRepliesRequest request);
}