using System.Collections.Immutable;

namespace Threddit.Application.DTOs.Requests.SubThreads;

public sealed record EditSubThreadRuleRequest(
    Guid RuleId,
    Guid SubThreadId,
    Guid RequestingUserId,
    ImmutableHashSet<Guid> ModeratedSubThreadIds,
    ImmutableHashSet<Guid> OwnedSubThreadIds,
    string RuleTitle,
    string RuleContent
);