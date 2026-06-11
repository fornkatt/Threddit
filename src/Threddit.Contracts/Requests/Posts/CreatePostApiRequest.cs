using System.ComponentModel.DataAnnotations;

namespace Threddit.Contracts.Requests.Posts;

public sealed record CreatePostApiRequest(
    [MaxLength(100)] string Title,
    [MaxLength(3000)] string Content,
    [MaxLength(2048)] string? ImageUrl = null
);