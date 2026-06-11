namespace Threddit.Contracts.Requests.SubThreads;

public sealed record CreateSubThreadRuleApiRequest(
    string RuleTitle,
    string RuleContent
);