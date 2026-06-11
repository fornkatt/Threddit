namespace Threddit.Application.DTOs.Requests.SubThreads;

public sealed record SubscribeSubThreadRequest(
    string SubThreadName,
    Guid RequestingUserId
);