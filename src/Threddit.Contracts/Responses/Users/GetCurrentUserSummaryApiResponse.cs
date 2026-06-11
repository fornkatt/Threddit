using System.Collections.Immutable;
using Threddit.Contracts.Common;

namespace Threddit.Contracts.Responses.Users;

public sealed record GetCurrentUserSummaryApiResponse(
    Guid UserId,
    string Username,
    bool IsSiteOwner,
    bool IsSiteAdmin,
    IReadOnlyList<SubscribedSubThreadApiDto> Subscriptions,
    IReadOnlyList<Guid> OwnedSubThreadIds,
    IReadOnlyList<Guid> ModeratedSubThreadIds
);