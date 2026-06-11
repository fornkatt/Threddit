using Threddit.Domain.Common;

namespace Threddit.Application.DTOs.Responses.Posts;

public sealed record DeletePostResponse(
    bool IsSuccess,
    string? Message = null,
    ErrorType? ErrorType = null
);