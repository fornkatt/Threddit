namespace Threddit.Application.DTOs.Requests.Auth;

public sealed record RegistrationRequest(
    string Username,
    string Email,
    string Password
);