namespace Threddit.Application.DTOs.Requests.Comments;

public sealed record CreateCommentRequest(
    Guid RequestingUserId,
    Guid PostId,
    string Content,
    string? ImageUrl = null,
    Guid? ParentCommentId = null
);