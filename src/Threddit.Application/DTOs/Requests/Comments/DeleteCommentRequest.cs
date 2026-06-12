using System.Collections.Immutable;

namespace Threddit.Application.DTOs.Requests.Comments;

public sealed record DeleteCommentRequest(
    Guid CommentId,
    Guid RequestingUserId,
    bool IsSiteAdmin,
    bool IsSiteOwner,
    ImmutableHashSet<Guid> ModeratedSubThreadIds,
    ImmutableHashSet<Guid> OwnedSubThreads,
    string? Reason = null
);