using Threddit.Contracts.Responses.Comments;

namespace Threddit.Contracts.Responses.Posts;

public sealed record GetPostWithCommentsApiResponse(
    Guid Id,
    Guid SubThreadId,
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
    DateTime? UpdatedAt,
    IReadOnlyList<GetCommentApiResponse> Comments
);