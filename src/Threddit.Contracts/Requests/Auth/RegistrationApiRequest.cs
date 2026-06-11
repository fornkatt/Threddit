namespace Threddit.Contracts.Requests.Auth;

public sealed record RegistrationApiRequest(
    string Username,
    string Email,
    string Password
);