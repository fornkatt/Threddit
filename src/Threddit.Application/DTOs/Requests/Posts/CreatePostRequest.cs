namespace Threddit.Application.DTOs.Requests.Posts;

public sealed record CreatePostRequest(
    Guid RequestingUserId,
    string SubThreadName,
    string Title,
    string Content,
    string? ImageUrl = null
);