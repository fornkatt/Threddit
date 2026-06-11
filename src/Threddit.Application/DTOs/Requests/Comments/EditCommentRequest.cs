namespace Threddit.Application.DTOs.Requests.Comments;

public sealed record EditCommentRequest(
    Guid CommentId,
    Guid RequestingUserId,
    string NewContent,
    string? NewImageUrl = null
);