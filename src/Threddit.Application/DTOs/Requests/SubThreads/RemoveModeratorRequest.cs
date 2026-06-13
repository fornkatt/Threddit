namespace Threddit.Application.DTOs.Requests.SubThreads;

public sealed record RemoveModeratorRequest(
    string SubThreadName,
    Guid TargetUserId,
    Guid RequestingUserId
);