using Threddit.Domain.Common;

namespace Threddit.Application.DTOs.Responses.Posts;

public sealed record VotePostResponse(
    bool IsSuccess,
    int? NewScore = null,
    string? Message = null,
    ErrorType? ErrorType = null
);