using System.ComponentModel.DataAnnotations;

namespace Threddit.Contracts.Requests.Comments;

public sealed record CreateCommentApiRequest(
    [MaxLength(3000)] string Content,
    Guid? ParentCommentId = null,
    [MaxLength(2048)] string? ImageUrl = null
);