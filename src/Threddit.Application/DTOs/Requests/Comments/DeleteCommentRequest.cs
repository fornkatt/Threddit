using System.Collections.Immutable;

namespace Threddit.Application.DTOs.Requests.Comments;

public sealed record DeleteCommentRequest(
    Guid CommentId,
    Guid RequestingUserId,
    bool IsSiteAdmin,
    bool IsSiteOwner,
    ImmutableArray<Guid> ModeratedSubThreadIds,
    string? Reason = null
);