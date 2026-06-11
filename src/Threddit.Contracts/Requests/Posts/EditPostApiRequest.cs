using System.ComponentModel.DataAnnotations;

namespace Threddit.Contracts.Requests.Posts;

public sealed record EditPostApiRequest(
    [MaxLength(3000)] string NewContent,
    [MaxLength(2048)] string? NewImageUrl = null
);