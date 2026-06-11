namespace Threddit.Contracts.Responses.Auth;

public sealed record LoginApiResponse(
    string JwtToken,
    DateTime ExpiresAt,
    string Username
    );