namespace Threddit.Application.DTOs.Requests.Auth;

public sealed record LoginRequest(
    string UsernameOrEmail,
    string Password
);