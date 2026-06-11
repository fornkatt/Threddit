using Threddit.Application.DTOs.Common;

namespace Threddit.Application.DTOs.Responses.Posts;

public sealed record PostWithCommentsDto(
    PostDto Post,
    IReadOnlyList<CommentDto> Comments
);