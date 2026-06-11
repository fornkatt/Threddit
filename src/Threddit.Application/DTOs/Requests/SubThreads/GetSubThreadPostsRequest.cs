namespace Threddit.Application.DTOs.Requests.SubThreads;

public sealed record GetSubThreadPostsRequest(
    string SubThreadName,
    int Page = 1,
    int PageSize = 20,
    PostSortOrder SortOrder = PostSortOrder.New
);

public enum PostSortOrder
{
    New,
    Top,
    Hot
}