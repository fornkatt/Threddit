using Threddit.Domain.Common;

namespace Threddit.Application.DTOs.Responses.Posts;

public sealed record GetPostWithCommentsResponse(
    bool IsSuccess,
    PostWithCommentsDto? PostWithComments = null,
    string? Message = null,
    ErrorType? ErrorType = null
);