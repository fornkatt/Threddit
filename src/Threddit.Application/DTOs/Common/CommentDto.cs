using System.Collections.Immutable;

namespace Threddit.Application.DTOs.Common;

public sealed record CommentDto(
    Guid Id,
    Guid? CommentedById,
    string? CommentedByUsername,
    string? CommentedByProfilePicture,
    string? Content,
    string? ImageUrl,
    int Score,
    DateTime CommentedAt,
    DateTime? UpdatedAt,
    bool IsDeleted,
    ImmutableList<CommentDto> Replies,
    bool HasReplies
);