using Threddit.Domain.Entities;

namespace Threddit.Application.DTOs.Common;

public sealed record ReportDto(
    Guid Id,
    Report.ReportType Type,
    Report.ReportCategory Category,
    Report.ReportStatus Status,
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