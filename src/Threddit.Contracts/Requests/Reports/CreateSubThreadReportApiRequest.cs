namespace Threddit.Contracts.Requests.Reports;

/// <summary>
/// Valid types: "Post" and "Comment".
/// <br/><br/>
/// Valid categories: "Spam", "Harassment", "RuleViolation", "Other".
/// </summary>
public sealed record CreateSubThreadReportApiRequest(
    string Type,
    Guid TargetId,
    string Category,
    string? Message = null
);