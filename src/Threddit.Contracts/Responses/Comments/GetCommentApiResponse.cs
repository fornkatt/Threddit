namespace Threddit.Contracts.Responses.Comments;

public sealed record GetCommentApiResponse(
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
    IReadOnlyList<GetCommentApiResponse> Replies,
    bool HasReplies
);