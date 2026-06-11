namespace Threddit.Application.DTOs.Requests.SubThreads;

public sealed record GetSubThreadModeratorsRequest(
    string SubThreadName
);