namespace Threddit.Contracts.Responses.SubThreads;

public sealed record GetSubThreadModeratorsApiResponse(
    IReadOnlyList<SubThreadModeratorDto> Moderators
);

public sealed record SubThreadModeratorDto(
    Guid UserId,
    string Username
);