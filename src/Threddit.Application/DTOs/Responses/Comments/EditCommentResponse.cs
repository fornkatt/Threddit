using Threddit.Domain.Common;

namespace Threddit.Application.DTOs.Responses.Comments;

public sealed record EditCommentResponse(
    bool IsSuccess,
    string? Message = null,
    ErrorType? ErrorType = null
);