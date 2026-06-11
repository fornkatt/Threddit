using System.Collections.Immutable;

namespace Threddit.Application.DTOs.Requests.SubThreads;

public sealed record CreateSubThreadRuleRequest(
    string SubThreadName,
    Guid RequestingUserId,
    ImmutableArray<Guid> ModeratedSubThreadIds,
    string RuleTitle,
    string RuleContent
);