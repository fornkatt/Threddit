namespace Threddit.Contracts.Responses.Posts;

public sealed record CreatePostApiResponse(
    Guid Id,
    string? PostedByUsername,
    string? PostedByProfilePicture,
    string? Title,
    string? Content,
    string? ImageUrl,
    string Slug,
    int Score,
    int CommentCount,
    DateTime PostedAt
);