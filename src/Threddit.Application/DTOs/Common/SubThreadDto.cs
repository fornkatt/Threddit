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
    IReadOnlyList<SubThreadRuleDto> Rules
);