using System.Collections.Immutable;

namespace Threddit.Application.DTOs.Requests.SubThreads;

public sealed record EditSubThreadRuleRequest(
    Guid RuleId,
    Guid SubThreadId,
    Guid RequestingUserId,
    ImmutableArray<Guid> ModeratedSubThreadIds,
    string RuleTitle,
    string RuleContent
);