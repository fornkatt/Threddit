using System.Collections.Immutable;

namespace Threddit.Application.DTOs.Common;

public sealed record CurrentUserSummaryDto(
    Guid UserId,
    string Username,
    bool IsSiteOwner,
    bool IsSiteAdmin,
    ImmutableList<SubscribedSubThreadDto> Subscriptions,
    ImmutableList<Guid> OwnedSubThreadIds,
    ImmutableList<Guid> ModeratedSubThreadIds
);