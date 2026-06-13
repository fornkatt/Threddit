namespace Threddit.Contracts.Responses.Conversations;

public sealed record GetMessagesApiResponse(
    List<DirectMessageApiDto> Messages,
    int Page,
    int PageSize
);