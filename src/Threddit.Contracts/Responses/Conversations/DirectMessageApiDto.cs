namespace Threddit.Contracts.Responses.Conversations;

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