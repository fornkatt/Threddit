using Threddit.Domain.Common;

namespace Threddit.Application.DTOs.Responses.Comments;

public sealed record DeleteCommentResponse(
    bool IsSuccess,
    string? Message = null,
    ErrorType? ErrorType = null
);