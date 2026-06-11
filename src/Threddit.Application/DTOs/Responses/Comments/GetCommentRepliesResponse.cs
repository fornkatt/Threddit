using Threddit.Application.DTOs.Common;
using Threddit.Domain.Common;

namespace Threddit.Application.DTOs.Responses.Comments;

public sealed record GetCommentRepliesResponse(
    bool IsSuccess,
    IReadOnlyList<CommentDto>? Replies = null,
    string? Message = null,
    ErrorType? ErrorType = null
);