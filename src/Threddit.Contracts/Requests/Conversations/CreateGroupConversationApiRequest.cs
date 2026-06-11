namespace Threddit.Contracts.Requests.Conversations;

public sealed record CreateGroupConversationApiRequest(
    string Name,
    List<Guid> MemberIds
);