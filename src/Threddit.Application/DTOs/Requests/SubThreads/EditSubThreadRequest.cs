namespace Threddit.Application.DTOs.Requests.SubThreads;

public sealed record EditSubThreadRequest(
    string SubThreadName,
    Guid RequestingUserId,
    string? Description = null,
    string? BannerImageUrl = null
);