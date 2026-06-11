namespace Threddit.Application.DTOs.Requests.Comments;

public sealed record GetCommentRepliesRequest(
    Guid ParentCommentId
);