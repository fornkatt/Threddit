using Threddit.Application.DTOs.Common;
using Threddit.Domain.Common;

namespace Threddit.Application.DTOs.Responses.SubThreads;

public sealed record GetSubThreadPostsResponse(
    bool IsSuccess,
    PagedResult<PostDto>? Posts = null,
    string? Message = null,
    ErrorType? ErrorType = null
);