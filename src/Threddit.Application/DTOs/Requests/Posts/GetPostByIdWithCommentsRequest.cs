namespace Threddit.Application.DTOs.Requests.Posts;

public sealed record GetPostByIdWithCommentsRequest(
    Guid PostId
);