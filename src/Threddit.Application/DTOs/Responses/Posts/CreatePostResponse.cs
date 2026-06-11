using Threddit.Application.DTOs.Common;
using Threddit.Domain.Common;

namespace Threddit.Application.DTOs.Responses.Posts;

public sealed record CreatePostResponse(
    bool IsSuccess,
    PostDto? Post = null,
    string? Message = null,
    ErrorType? ErrorType = null
);