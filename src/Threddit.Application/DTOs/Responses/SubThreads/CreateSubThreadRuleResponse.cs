using Threddit.Domain.Common;

namespace Threddit.Application.DTOs.Responses.SubThreads;

public sealed record CreateSubThreadRuleResponse(
    bool IsSuccess,
    Guid? RuleId = null,
    string? Message = null,
    ErrorType? ErrorType = null
);