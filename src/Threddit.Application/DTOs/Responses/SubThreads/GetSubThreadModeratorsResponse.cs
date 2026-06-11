using System.Collections.Immutable;
using Threddit.Application.DTOs.Common;
using Threddit.Domain.Common;

namespace Threddit.Application.DTOs.Responses.SubThreads;

public sealed record GetSubThreadModeratorsResponse(
    bool IsSuccess,
    ImmutableList<SubThreadModeratorDto> Moderators,
    string? Message = null,
    ErrorType? ErrorType = null
);