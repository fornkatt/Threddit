using System.Collections.Immutable;

namespace Threddit.Application.DTOs.Requests.Posts;

public sealed record DeletePostRequest(
    Guid PostId,
    Guid RequestingUserId,
    bool IsSiteAdmin,
    bool IsSiteOwner,
    ImmutableHashSet<Guid> ModeratedSubThreadIds,
    ImmutableHashSet<Guid> OwnedSubThreads,
    string? Reason = null
);