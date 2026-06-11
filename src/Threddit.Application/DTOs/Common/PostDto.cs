namespace Threddit.Application.DTOs.Common;

public sealed record PostDto(
    Guid Id,
    Guid SubThreadId,
    Guid? PostedById,
    string? PostedByUsername,
    string? PostedByProfilePicture,
    string Title,
    string? Content,
    string? ImageUrl,
    string? Slug,
    int Score,
    int CommentCount,
    DateTime PostedAt,
    DateTime? UpdatedAt
);