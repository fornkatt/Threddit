namespace Threddit.Contracts.Responses.SubThreads;

public sealed record GetSubThreadSearchApiResponse(
    Guid Id,
    string Name,
    int SubscriberCount
);