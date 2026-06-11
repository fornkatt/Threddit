using Threddit.Domain.Common;

namespace Threddit.Application.DTOs.Responses.Comments;

public sealed record VoteCommentResponse(
    bool IsSuccess,
    int? NewScore = null,
    string? Message = null,
    ErrorType? ErrorType = null
);