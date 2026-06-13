namespace Threddit.Contracts.Responses.Conversations;

public sealed record GetConversationsApiResponse(
    IReadOnlyList<ConversationApiDto> Conversations,
    IReadOnlyList<GroupConversationApiDto> GroupConversations
);