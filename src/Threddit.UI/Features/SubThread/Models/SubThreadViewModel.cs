using System.Collections.Immutable;
using Threddit.Contracts.Responses.SubThreads;

namespace Threddit.UI.Features.SubThread.Models;

public sealed class SubThreadViewModel
{
    public Guid Id { get; init; }
    public string Name { get; private set; } = string.Empty;
    public string? Description { get; init; }
    public string? BannerImageUrl { get; private init; }
    public int SubscriberCount { get; init; }
    public string? CreatedByUsername { get; init; }
    public DateTime CreatedAt { get; init; }
    public IReadOnlyList<GetSubThreadRuleApiResponse> Rules { get; init; } = [];

    public static SubThreadViewModel FromResponse(GetSubThreadApiResponse response) => new()
    {
        Id = response.Id,
        Name = response.Name,
        Description = response.Description,
        BannerImageUrl = response.BannerImageUrl,
        SubscriberCount = response.SubscriberCount,
        CreatedByUsername = response.CreatedByUsername,
        CreatedAt = response.CreatedAt,
        Rules = response.Rules
    };
}