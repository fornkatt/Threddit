namespace Threddit.Application.DTOs.Requests.Posts;

public sealed record VotePostRequest(
    Guid PostId,
    Guid RequestingUserId,
    bool IsUpvote
);