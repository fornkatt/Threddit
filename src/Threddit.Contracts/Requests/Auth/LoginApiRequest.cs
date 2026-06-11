namespace Threddit.Contracts.Requests.Auth;

public sealed record LoginApiRequest(
    string UsernameOrEmail,
    string Password
);