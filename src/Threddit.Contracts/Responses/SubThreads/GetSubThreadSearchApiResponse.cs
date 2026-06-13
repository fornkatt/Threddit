namespace Threddit.Contracts.Responses.SubThreads;

public sealed record GetSubThreadSearchApiResponse(
    string Name,
    int SubscriberCount
);