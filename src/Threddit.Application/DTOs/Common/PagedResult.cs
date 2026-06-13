using System.Collections.Immutable;

namespace Threddit.Application.DTOs.Common;

public sealed record PagedResult<T>(
    ImmutableList<T> Items,
    int Page,
    int PageSize,
    int TotalCount
)
{
    public bool HasNextPage => Page * PageSize < TotalCount;
    public bool HasPreviousPage => Page > 1;
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
}