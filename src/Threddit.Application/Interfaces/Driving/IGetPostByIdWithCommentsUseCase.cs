using Threddit.Application.DTOs.Requests.Posts;
using Threddit.Application.DTOs.Responses.Posts;
using Threddit.Domain.Common;

namespace Threddit.Application.Interfaces.Driving;

public interface IGetPostByIdWithCommentsUseCase
{
    /// <summary>Requests a post by GUID along with a set of comments and replies.</summary>
    /// <remarks>
    /// Possible error types:
    /// <list type="bullet">
    ///     <item><see cref="ErrorType.PostNotFound"/></item>
    ///     <item><see cref="ErrorType.DatabaseTimeout"/></item>
    /// </list>
    /// </remarks>
    Task<GetPostWithCommentsResponse> ExecuteAsync(GetPostByIdWithCommentsRequest request);
}