using System.Collections.Immutable;

namespace Threddit.Application.DTOs.Requests.Users;

public sealed record BanSubThreadUserRequest(
    Guid TargetUserId,
    string SubThreadName,
    Guid RequestingUserId,
    ImmutableArray<Guid> ModeratedSubThreadIds,
    string Reason,
    DateTime ExpiresAt
);