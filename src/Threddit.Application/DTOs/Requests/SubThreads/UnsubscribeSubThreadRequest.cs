namespace Threddit.Application.DTOs.Requests.SubThreads;

public sealed record UnsubscribeSubThreadRequest(
    string SubThreadName,
    Guid RequestingUserId
);