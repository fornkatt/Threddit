namespace Threddit.Contracts.Requests.SubThreads;

public sealed record CreateSubThreadApiRequest(
    string Name,
    string? Description = null,
    string? BannerUrl = null
);