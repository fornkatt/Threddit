namespace Threddit.Contracts.Responses.SubThreads;

public sealed record GetSubThreadPostApiResponse(
    Guid Id,
    Guid? PostedById,
    string? PostedByUsername,
    string? PostedByProfilePicture,
    string Title,
    string? Content,
    string? ImageUrl,
    string Slug,
    int Score,
    int CommentCount,
    DateTime PostedAt,
    DateTime? UpdatedAt
);