namespace Threddit.Contracts.Requests.SubThreads;

public sealed record EditSubThreadApiRequest(
    string? Description,
    string? BannerImageUrl
);