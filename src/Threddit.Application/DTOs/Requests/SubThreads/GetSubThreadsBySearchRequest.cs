namespace Threddit.Application.DTOs.Requests.SubThreads;

public sealed record GetSubThreadsBySearchRequest(
    string Query,
    int Page = 1,
    int PageSize = 30
);