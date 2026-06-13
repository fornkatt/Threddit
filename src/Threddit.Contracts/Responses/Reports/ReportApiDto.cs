namespace Threddit.Contracts.Responses.Reports;

public sealed record ReportApiDto(
    Guid Id,
    string Type,
    string Category,
    string Status,
    string? Message,
    Guid? ReporterId,
    string? ReporterUsername,
    Guid? SubThreadId,
    string? SubThreadName,
    Guid? TargetPostId,
    string? TargetPostTitle,
    Guid? TargetCommentId,
    string? TargetCommentContent,
    Guid? TargetUserId,
    string? TargetUsername,
    Guid? TargetSubThreadId,
    string? TargetSubThreadName,
    Guid? TargetDirectMessageId,
    string? TargetDirectMessageContent,
    DateTime ReportedAt
);