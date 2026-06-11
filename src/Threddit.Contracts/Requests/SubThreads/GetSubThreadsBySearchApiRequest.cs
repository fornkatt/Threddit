namespace Threddit.Contracts.Requests.SubThreads;

public sealed record GetSubThreadsBySearchApiRequest(
    string Query = "",
    int Page = 1,
    int PageSize = 20
);