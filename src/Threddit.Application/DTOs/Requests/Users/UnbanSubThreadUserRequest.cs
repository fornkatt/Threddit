using System.Collections.Immutable;

namespace Threddit.Application.DTOs.Requests.Users;

public sealed record UnbanSubThreadUserRequest(
    Guid TargetUserId,
    string SubThreadName,
    Guid RequestingUserId,
    ImmutableHashSet<Guid> ModeratedSubThreadIds,
    ImmutableHashSet<Guid> OwnedSubThreadIds
);