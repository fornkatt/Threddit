namespace Threddit.Contracts.Responses.Users;

public sealed record GetUserProfileApiResponse(
    Guid Id,
    string Username,
    string? ProfilePicture,
    string? Description,
    int PostScore,
    int CommentScore,
    int TotalScore,
    DateTime CreationDate
);