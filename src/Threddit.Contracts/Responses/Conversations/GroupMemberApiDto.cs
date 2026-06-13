namespace Threddit.Contracts.Responses.Conversations;

public sealed record GroupMemberApiDto(
    Guid? UserId,
    string? Username,
    string? ProfilePicture,
    bool HasLeft = false
);