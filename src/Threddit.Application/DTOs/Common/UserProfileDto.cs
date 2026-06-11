namespace Threddit.Application.DTOs.Common;

public sealed record UserProfileDto(
    Guid Id,
    string Username,
    string? ProfilePicture,
    string? Description,
    int PostScore,
    int CommentScore,
    int TotalScore,
    DateTime CreationDate
);