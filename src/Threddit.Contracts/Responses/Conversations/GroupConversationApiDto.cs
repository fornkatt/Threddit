namespace Threddit.Contracts.Responses.Conversations;

public sealed record GroupConversationApiDto(
    Guid Id,
    string Name,
    Guid? CreatedById,
    IReadOnlyList<GroupMemberApiDto> Members,
    DateTime CreatedAt
);