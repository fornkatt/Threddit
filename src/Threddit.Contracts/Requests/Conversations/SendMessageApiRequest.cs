namespace Threddit.Contracts.Requests.Conversations;

public sealed record SendMessageApiRequest(
    string Content,
    Guid? ParentMessageId = null
);