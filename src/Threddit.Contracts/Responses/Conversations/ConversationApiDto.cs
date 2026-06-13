namespace Threddit.Contracts.Responses.Conversations;

public sealed record ConversationApiDto(
    Guid Id,
    string? OtherUsername,
    string? OtherProfilePicture,
    DateTime CreatedAt
);