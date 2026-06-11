using Threddit.Application.DTOs.Common;
using Threddit.Domain.Common;

namespace Threddit.Application.DTOs.Responses.SubThreads;

public sealed record GetSubThreadByNameResponse(
    bool IsSuccess,
    SubThreadDto? SubThread = null,
    string? Message = null,
    ErrorType? ErrorType = null
);