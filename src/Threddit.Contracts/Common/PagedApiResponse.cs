namespace Threddit.Contracts.Common;

public sealed record PagedApiResponse<T>(
    IReadOnlyList<T> Items,
    int Page,
    int PageSize,
    int TotalCount,
    bool HasNextPage
);