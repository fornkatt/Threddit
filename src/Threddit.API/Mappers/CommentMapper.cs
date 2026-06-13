using System.Collections.Immutable;
using Threddit.Application.DTOs.Common;
using Threddit.Contracts.Responses.Comments;

namespace Threddit.API.Mappers;

public static class CommentMapper
{
    public static ImmutableList<GetCommentApiResponse> MapComments(IReadOnlyList<CommentDto> comments) =>
        comments.Select(c => new GetCommentApiResponse(
            c.Id,
            c.CommentedById,
            c.CommentedByUsername,
            c.CommentedByProfilePicture,
            c.Content,
            c.ImageUrl,
            c.Score,
            c.CommentedAt,
            c.UpdatedAt,
            c.IsDeleted,
            MapComments(c.Replies),
            c.HasReplies
        )).ToImmutableList();
}