using System.Collections.Immutable;
using Threddit.Application.DTOs.Common;
using Threddit.Domain.Entities;

namespace Threddit.Application.Mappers;

public static class CommentMapper
{
    public static ImmutableList<CommentDto> MapComments(IEnumerable<Comment> comments) =>
        comments.Select(c => new CommentDto(
            c.Id,
            c.CommentedById,
            c.CommentedBy?.UserName,
            c.CommentedBy?.ProfilePicture,
            c.Content,
            c.ImageUrl,
            c.Score,
            c.CommentedAt,
            c.UpdatedAt,
            c.IsDeleted,
            MapComments(c.Replies),
            c.ReplyCount > 0
        )).ToImmutableList();
}