namespace Threddit.Application.DTOs.Requests.Comments;

public sealed record VoteCommentRequest(
    Guid CommentId,
    Guid RequestingUserId,
    bool IsUpvote
);