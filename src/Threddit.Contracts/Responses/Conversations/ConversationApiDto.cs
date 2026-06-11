namespace Threddit.Contracts.Responses.Conversations;

public sealed record ConversationApiDto(
    Guid Id,
    string? OtherUsername,
    string? OtherProfilePicture,
    DateTime CreatedAt
);

public sealed record GroupConversationApiDto(
    Guid Id,
    string Name,
    IReadOnlyList<GroupMemberApiDto> Members,
    DateTime CreatedAt
);

public sealed record GroupMemberApiDto(
    Guid? UserId,
    string? Username,
    string? ProfilePicture,
    bool HasLeft = false
);

public sealed record DirectMessageApiDto(
    Guid Id,
    Guid? SenderId,
    string? SenderUsername,
    string? SenderProfilePicture,
    string? Content,
    bool IsDeleted,
    bool IsReply,
    Guid? ParentMessageId,
    string? ParentMessageContent,
    string? ParentMessageSenderUsername,
    DateTime SentAt,
    DateTime? EditedAt
);

public sealed record GetConversationsApiResponse(
    IReadOnlyList<ConversationApiDto> Conversations,
    IReadOnlyList<GroupConversationApiDto> GroupConversations
);

public sealed record GetMessagesApiResponse(
    List<DirectMessageApiDto> Messages,
    int Page,
    int PageSize
);

public sealed record CreateConversationApiResponse(Guid ConversationId);
public sealed record CreateGroupConversationApiResponse(Guid GroupConversationId);
public sealed record SendMessageApiResponse(DirectMessageApiDto Message);