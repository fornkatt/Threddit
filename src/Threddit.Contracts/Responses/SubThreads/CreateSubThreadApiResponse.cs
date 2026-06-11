namespace Threddit.Contracts.Responses.SubThreads;

public sealed record CreateSubThreadApiResponse(
    Guid Id,
    string Name,
    string? Description,
    string? BannerImageUrl,
    int SubscriberCount,
    DateTime CreatedAt
);