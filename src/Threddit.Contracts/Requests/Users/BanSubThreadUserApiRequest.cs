namespace Threddit.Contracts.Requests.Users;

public sealed record BanSubThreadUserApiRequest(
    string Reason,
    DateTime ExpiresAt
);