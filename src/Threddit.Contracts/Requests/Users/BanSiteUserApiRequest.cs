namespace Threddit.Contracts.Requests.Users;

public sealed record BanSiteUserApiRequest(
    string Reason,
    DateTime ExpiresAt
);