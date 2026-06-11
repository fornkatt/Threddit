using System.ComponentModel.DataAnnotations;

namespace Threddit.Contracts.Requests.Comments;

public sealed record EditCommentApiRequest(
    [MaxLength(3000)] string NewContent,
    [MaxLength(2048)] string? NewImageUrl = null
);