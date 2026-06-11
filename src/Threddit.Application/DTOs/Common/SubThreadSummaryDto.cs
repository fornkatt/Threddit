namespace Threddit.Application.DTOs.Common;

public sealed record SubThreadSummaryDto(
    Guid Id,
    string Name,
    int SubscriberCount
);