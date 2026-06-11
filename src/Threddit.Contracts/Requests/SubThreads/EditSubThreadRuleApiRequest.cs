namespace Threddit.Contracts.Requests.SubThreads;

public sealed record EditSubThreadRuleApiRequest(
    string RuleTitle,
    string RuleContent
);