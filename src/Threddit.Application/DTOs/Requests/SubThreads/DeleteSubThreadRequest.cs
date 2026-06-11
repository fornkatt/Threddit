namespace Threddit.Application.DTOs.Requests.SubThreads;

public sealed record DeleteSubThreadRequest(
    Guid SubThreadId,
    Guid RequestingUserId,
    bool IsSiteAdmin,
    bool IsSiteOwner
);