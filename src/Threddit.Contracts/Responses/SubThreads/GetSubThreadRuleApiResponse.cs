namespace Threddit.Contracts.Responses.SubThreads;

public sealed record GetSubThreadRuleApiResponse(
    Guid Id,
    string Title,
    string Content
);