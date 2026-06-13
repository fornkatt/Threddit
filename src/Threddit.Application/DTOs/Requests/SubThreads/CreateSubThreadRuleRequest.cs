using System.Collections.Immutable;

namespace Threddit.Application.DTOs.Requests.SubThreads;

public sealed record CreateSubThreadRuleRequest(
    string SubThreadName,
    Guid RequestingUserId,
    ImmutableHashSet<Guid> ModeratedSubThreadIds,
    ImmutableHashSet<Guid> OwnedSubThreadIds,
    string RuleTitle,
    string RuleContent
);