using System.Collections.Immutable;

namespace Threddit.Application.DTOs.Requests.Posts;

public sealed record DeletePostRequest(
    Guid PostId,
    Guid RequestingUserId,
    bool IsSiteAdmin,
    bool IsSiteOwner,
    ImmutableArray<Guid> ModeratedSubThreadIds,
    string? Reason = null
);