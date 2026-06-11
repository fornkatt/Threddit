namespace Threddit.Application.DTOs.Requests.SubThreads;

public sealed record CreateSubThreadRequest(
    Guid RequestingUserId,
    string Name,
    string? Description = null,
    string? BannerImageUrl = null
);