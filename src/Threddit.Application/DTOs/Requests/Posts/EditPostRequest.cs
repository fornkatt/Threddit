namespace Threddit.Application.DTOs.Requests.Posts;

public sealed record EditPostRequest(
    Guid PostId,
    Guid RequestingUserId,
    string NewContent,
    string? NewImageUrl = null
);