namespace Threddit.Contracts.Requests.Reports;

/// <summary>
/// Valid types: "User", "SubThread" and "DirectMessage".
/// <br/><br/>
/// Valid categories: "Spam", "Harassment", "RuleViolation", "Other".
/// </summary>
public sealed record CreateSiteReportApiRequest(
    string Type,
    Guid TargetId,
    string Category,
    string? Message = null
);