using System.Collections.Immutable;

namespace Threddit.Application.DTOs.Common;

public sealed record SubThreadDto(
    Guid Id,
    string? CreatedByUsername,
    string Name,
    string? Description,
    string? BannerImageUrl,
    int SubscriberCount,
    DateTime CreatedAt,
    DateTime? UpdatedAt,
    ImmutableList<SubThreadRuleDto> Rules
);