using System.Collections.Immutable;
using Threddit.Application.DTOs.Common;
using Threddit.Domain.Common;

namespace Threddit.Application.DTOs.Responses.Users;

public sealed record GetAllSiteAdminsResponse(
    bool IsSuccess,
    ImmutableList<SiteAdminDto> SiteAdmins,
    string? Message = null,
    ErrorType? ErrorType = null
);