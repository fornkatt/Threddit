namespace Threddit.Application.DTOs.Common;

public sealed record SubThreadRuleDto(
    Guid Id,
    string Title,
    string Content
);