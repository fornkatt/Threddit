namespace Threddit.Contracts.Responses.Comments;

public sealed record CreateCommentApiResponse(
    Guid Id,
    Guid? CommentedById,
    string? CommentedByUsername,
    string? CommentedByProfilePicture,
    string? Content,
    string? ImageUrl,
    int Score,
    DateTime CommentedAt,
    bool IsDeleted,
    Guid? ParentCommentId,
    bool HasReplies
);