namespace Threddit.Contracts.Requests.Users;

public sealed record EditUserProfileApiRequest(
    string? ProfilePicture = null,
    string? Description = null
);