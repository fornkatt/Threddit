using Threddit.Application.DTOs.Common;
using Threddit.Domain.Common;

namespace Threddit.Application.DTOs.Responses.Comments;

public sealed record CreateCommentResponse(
    bool IsSuccess,
    CommentDto? Comment = null,
    string? Message = null,
    ErrorType? ErrorType = null
);