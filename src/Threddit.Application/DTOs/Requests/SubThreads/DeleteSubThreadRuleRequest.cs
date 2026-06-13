using System.Collections.Immutable;

namespace Threddit.Application.DTOs.Requests.SubThreads;

public sealed record DeleteSubThreadRuleRequest(
    Guid RuleId,
    Guid SubThreadId,
    Guid RequestingUserId,
    ImmutableHashSet<Guid> ModeratedSubThreadIds,
    ImmutableHashSet<Guid> OwnedSubThreadIds
);