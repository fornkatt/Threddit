namespace Threddit.Contracts.Responses.SubThreads;

public sealed record GetSubThreadApiResponse(
    Guid Id,
    string? CreatedByUsername,
    string Name,
    string? Description,
    string? BannerImageUrl,
    int SubscriberCount,
    DateTime CreatedAt,
    DateTime? UpdatedAt,
    IReadOnlyList<GetSubThreadRuleApiResponse> Rules
);