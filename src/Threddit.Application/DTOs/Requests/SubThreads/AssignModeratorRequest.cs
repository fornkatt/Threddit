namespace Threddit.Application.DTOs.Requests.SubThreads;

public sealed record AssignModeratorRequest(
    string SubThreadName,
    Guid TargetUserId,
    Guid RequestingUserId
);