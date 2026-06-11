namespace Threddit.Application.DTOs.Common;

public sealed record SubThreadModeratorDto(
    Guid UserId,
    string Username
);