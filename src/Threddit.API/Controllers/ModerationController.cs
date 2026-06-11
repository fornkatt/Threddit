using System.Collections.Immutable;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Threddit.Application.DTOs.Requests.Reports;
using Threddit.Application.DTOs.Requests.SubThreads;
using Threddit.Application.DTOs.Requests.Users;
using Threddit.Application.Interfaces.Driving;
using Threddit.Contracts.Common;
using Threddit.Contracts.Requests.Reports;
using Threddit.Contracts.Requests.Users;
using Threddit.Contracts.Responses.Reports;
using Threddit.Contracts.Responses.Users;
using Threddit.Domain.Common;
using Threddit.Domain.Entities;

namespace Threddit.API.Controllers;

[ApiController]
[Route("api/moderation")]
public class ModerationController : ControllerBase
{
    private readonly IBanSiteUserUseCase _banSiteUserUseCase;
    private readonly IUnbanSiteUserUseCase _unbanSiteUserUseCase;
    private readonly IBanSubThreadUserUseCase _banSubThreadUserUseCase;
    private readonly IUnbanSubThreadUserUseCase _unbanSubThreadUserUseCase;
    private readonly IAssignModeratorUseCase _assignModeratorUseCase;
    private readonly IRemoveModeratorUseCase _removeModeratorUseCase;
    private readonly ICreateSubThreadReportUseCase _createSubThreadReportUseCase;
    private readonly ICreateSiteReportUseCase _createSiteReportUseCase;
    private readonly IGetSubThreadReportsUseCase _getSubThreadReportsUseCase;
    private readonly IGetSiteReportsUseCase _getSiteReportsUseCase;
    private readonly ISetReportStatusUseCase _setReportStatusUseCase;
    private readonly IAssignSiteAdminUseCase _assignSiteAdminUseCase;
    private readonly IRemoveSiteAdminUseCase _removeSiteAdminUseCase;
    private readonly IGetAllSiteAdminsUseCase _getAllSiteAdminsUseCase;

    public ModerationController(
        IBanSiteUserUseCase banSiteUserUseCase,
        IUnbanSiteUserUseCase unbanSiteUserUseCase,
        IBanSubThreadUserUseCase banSubThreadUserUseCase,
        IUnbanSubThreadUserUseCase unbanSubThreadUserUseCase,
        IAssignModeratorUseCase assignModeratorUseCase,
        IRemoveModeratorUseCase removeModeratorUseCase,
        ICreateSubThreadReportUseCase createSubThreadReportUseCase,
        ICreateSiteReportUseCase createSiteReportUseCase,
        IGetSubThreadReportsUseCase getSubThreadReportsUseCase,
        IGetSiteReportsUseCase getSiteReportsUseCase,
        ISetReportStatusUseCase setReportStatusUseCase,
        IAssignSiteAdminUseCase assignSiteAdminUseCase,
        IRemoveSiteAdminUseCase removeSiteAdminUseCase,
        IGetAllSiteAdminsUseCase getAllSiteAdminsUseCase
    )
    {
        _banSiteUserUseCase = banSiteUserUseCase;
        _unbanSiteUserUseCase = unbanSiteUserUseCase;
        _banSubThreadUserUseCase = banSubThreadUserUseCase;
        _unbanSubThreadUserUseCase = unbanSubThreadUserUseCase;
        _assignModeratorUseCase = assignModeratorUseCase;
        _removeModeratorUseCase = removeModeratorUseCase;
        _createSubThreadReportUseCase = createSubThreadReportUseCase;
        _createSiteReportUseCase = createSiteReportUseCase;
        _getSubThreadReportsUseCase = getSubThreadReportsUseCase;
        _getSiteReportsUseCase = getSiteReportsUseCase;
        _setReportStatusUseCase = setReportStatusUseCase;
        _assignSiteAdminUseCase = assignSiteAdminUseCase;
        _removeSiteAdminUseCase = removeSiteAdminUseCase;
        _getAllSiteAdminsUseCase = getAllSiteAdminsUseCase;
    }

    [HttpPost("users/{targetUserId:guid}/admin")]
    [Authorize(Policy = "SiteOwnerOnly")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status504GatewayTimeout)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> AssignSiteAdmin(
        [FromRoute] Guid targetUserId)
    {
        var result = await _assignSiteAdminUseCase.ExecuteAsync(new AssignSiteAdminRequest(
            targetUserId
        ));

        if (!result.IsSuccess)
            return result.ErrorType switch
            {
                ErrorType.UserNotFound => NotFound(new ErrorResponse(result.Message)),
                ErrorType.AlreadyAssigned or ErrorType.ConcurrencyFailure =>
                    Conflict(new ErrorResponse(result.Message)),
                ErrorType.DatabaseTimeout => StatusCode(504, new ErrorResponse(result.Message)),
                _ => StatusCode(500, new ErrorResponse(result.Message))
            };

        return NoContent();
    }

    [HttpDelete("users/{targetUserId:guid}/admin")]
    [Authorize(Policy = "SiteOwnerOnly")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status504GatewayTimeout)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> RemoveSiteAdmin(
        [FromRoute] Guid targetUserId)
    {
        var result = await _removeSiteAdminUseCase.ExecuteAsync(new RemoveSiteAdminRequest(
            targetUserId
        ));

        if (!result.IsSuccess)
            return result.ErrorType switch
            {
                ErrorType.UserNotFound or ErrorType.SiteAdminNotFound => NotFound(new ErrorResponse(result.Message)),
                ErrorType.ConcurrencyFailure => Conflict(new ErrorResponse(result.Message)),
                ErrorType.DatabaseTimeout => StatusCode(504, new ErrorResponse(result.Message)),
                _ => StatusCode(500, new ErrorResponse(result.Message))
            };

        return NoContent();
    }

    [HttpGet("admins")]
    [Authorize(Policy = "SiteOwnerOnly")]
    [ProducesResponseType(typeof(GetAllSiteAdminsApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status504GatewayTimeout)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<GetAllSiteAdminsApiResponse>> GetAllSiteAdmins()
    {
        var result = await _getAllSiteAdminsUseCase.ExecuteAsync();
        if (!result.IsSuccess)
            return result.ErrorType switch
            {
                ErrorType.DatabaseTimeout => StatusCode(504, new ErrorResponse(result.Message)),
                _ => StatusCode(500, new ErrorResponse(result.Message))
            };

        return Ok(new GetAllSiteAdminsApiResponse(
            result.SiteAdmins.Select(sa => new SiteAdminApiDto(
                sa.Id,
                sa.Username,
                sa.AssignedAt,
                sa.IssuedBans.Select(b => new IssuedSiteBanApiDto(
                    b.BanId,
                    b.BannedUserId,
                    b.BannedUsername,
                    b.Reason,
                    b.BannedAt,
                    b.ExpiresAt,
                    b.IsExpired
                )).ToImmutableList()
            )).ToImmutableList()
        ));
    }

    [HttpPost("users/{targetUserId:guid}/ban")]
    [Authorize(Policy = "SiteAdminOrOwner")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status504GatewayTimeout)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> BanSiteUser(
        [FromRoute] Guid targetUserId,
        [FromBody] BanSiteUserApiRequest apiRequest)
    {
        var requestingUserId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var result = await _banSiteUserUseCase.ExecuteAsync(new BanSiteUserRequest(
            targetUserId,
            requestingUserId,
            apiRequest.Reason,
            apiRequest.ExpiresAt
        ));

        if (!result.IsSuccess)
            return result.ErrorType switch
            {
                ErrorType.UserNotFound => NotFound(new ErrorResponse(result.Message)),
                ErrorType.BanReasonEmpty or ErrorType.BanReasonTooLong or ErrorType.InvalidBanDate
                    => BadRequest(new ErrorResponse(result.Message)),
                ErrorType.ConcurrencyFailure => Conflict(new ErrorResponse(result.Message)),
                ErrorType.DatabaseTimeout => StatusCode(504, new ErrorResponse(result.Message)),
                _ => StatusCode(500, new ErrorResponse(result.Message))
            };

        return NoContent();
    }

    [HttpDelete("users/{targetUserId:guid}/ban")]
    [Authorize(Policy = "SiteAdminOrOwner")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status504GatewayTimeout)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> UnbanSiteUser([FromRoute] Guid targetUserId)
    {
        var requestingUserId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var result = await _unbanSiteUserUseCase.ExecuteAsync(new UnbanSiteUserRequest(
            targetUserId,
            requestingUserId
        ));

        if (!result.IsSuccess)
            return result.ErrorType switch
            {
                ErrorType.UserNotFound or ErrorType.NotFound => NotFound(new ErrorResponse(result.Message)),
                ErrorType.Forbidden => StatusCode(403, new ErrorResponse(result.Message)),
                ErrorType.ConcurrencyFailure => Conflict(new ErrorResponse(result.Message)),
                ErrorType.DatabaseTimeout => StatusCode(504, new ErrorResponse(result.Message)),
                _ => StatusCode(500, new ErrorResponse(result.Message))
            };

        return NoContent();
    }

    [HttpPost("reports")]
    [Authorize(Policy = "NotBanned")]
    [ProducesResponseType(typeof(CreateReportApiResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status504GatewayTimeout)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<CreateReportApiResponse>> CreateSiteReport(
        [FromBody] CreateSiteReportApiRequest apiRequest)
    {
        if (!TryParseReportType(apiRequest.Type, out var reportType))
            return BadRequest(new ErrorResponse($"Invalid report type '{apiRequest.Type}'."));

        if (!TryParseReportCategory(apiRequest.Category, out var reportCategory))
            return BadRequest(new ErrorResponse($"Invalid report category '{apiRequest.Category}'."));

        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var result = await _createSiteReportUseCase.ExecuteAsync(new CreateSiteReportRequest(
            userId,
            reportType,
            apiRequest.TargetId,
            reportCategory,
            apiRequest.Message
        ));

        if (!result.IsSuccess)
            return result.ErrorType switch
            {
                ErrorType.UserNotFound or ErrorType.SubThreadNotFound or ErrorType.NotFound
                    => NotFound(new ErrorResponse(result.Message)),
                ErrorType.ContentTooLong => BadRequest(new ErrorResponse(result.Message)),
                ErrorType.DatabaseTimeout => StatusCode(504, new ErrorResponse(result.Message)),
                _ => StatusCode(500, new ErrorResponse(result.Message))
            };

        return StatusCode(201, new CreateReportApiResponse(result.ReportId!.Value));
    }

    [HttpGet("reports")]
    [Authorize(Policy = "SiteAdminOrOwner")]
    [ProducesResponseType(typeof(GetReportsApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status504GatewayTimeout)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<GetReportsApiResponse>> GetSiteReports(
        [FromQuery] string? status = null,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20
    )
    {
        Report.ReportStatus? parsedStatus = null;
        if (status is not null)
        {
            if (!TryParseReportStatus(status, out var reportStatus))
                return BadRequest(new ErrorResponse($"Invalid status filter '{status}'."));
            parsedStatus = reportStatus;
        }

        var result = await _getSiteReportsUseCase.ExecuteAsync(new GetSiteReportsRequest(
            parsedStatus,
            page,
            pageSize
        ));

        if (!result.IsSuccess)
            return result.ErrorType switch
            {
                ErrorType.Forbidden => StatusCode(403, new ErrorResponse(result.Message)),
                ErrorType.DatabaseTimeout => StatusCode(504, new ErrorResponse(result.Message)),
                _ => StatusCode(500, new ErrorResponse(result.Message))
            };

        return Ok(new GetReportsApiResponse(
            result.Reports.Select(r => new ReportApiDto(
                r.Id, r.Type.ToString(), r.Category.ToString(), r.Status.ToString(), r.Message,
                r.ReporterId, r.ReporterUsername,
                r.SubThreadId, r.SubThreadName,
                r.TargetPostId, r.TargetPostTitle,
                r.TargetCommentId, r.TargetCommentContent,
                r.TargetUserId, r.TargetUsername,
                r.TargetSubThreadId, r.TargetSubThreadName,
                r.TargetDirectMessageId, r.TargetDirectMessageContent,
                r.ReportedAt)).ToImmutableList()
        ));
    }

    [HttpPatch("reports/{reportId:guid}/status")]
    [Authorize(Policy = "SiteAdminOrOwner")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status504GatewayTimeout)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> SetSiteReportStatus(
        [FromRoute] Guid reportId,
        [FromBody] SetReportStatusApiRequest apiRequest)
    {
        if (!TryParseReportStatus(apiRequest.NewStatus, out var newStatus))
            return BadRequest(new ErrorResponse($"Invalid status '{apiRequest.NewStatus}'."));

        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var result = await _setReportStatusUseCase.ExecuteAsync(new SetReportStatusRequest(
            reportId,
            userId,
            true,
            true,
            [],
            newStatus
        ));

        if (!result.IsSuccess)
            return result.ErrorType switch
            {
                ErrorType.ReportNotFound => NotFound(new ErrorResponse(result.Message)),
                ErrorType.SameStatus => BadRequest(new ErrorResponse(result.Message)),
                ErrorType.ConcurrencyFailure => Conflict(new ErrorResponse(result.Message)),
                ErrorType.DatabaseTimeout => StatusCode(504, new ErrorResponse(result.Message)),
                _ => StatusCode(500, new ErrorResponse(result.Message))
            };

        return NoContent();
    }

    [HttpPost("subthreads/{subThreadName}/users/{targetUserId:guid}/ban")]
    [Authorize(Policy = "NotBanned")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status504GatewayTimeout)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> BanSubThreadUser(
        [FromRoute] string subThreadName,
        [FromRoute] Guid targetUserId,
        [FromBody] BanSubThreadUserApiRequest apiRequest)
    {
        var requestingUserId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var moderatedSubThreadIds = User.FindAll("moderator")
            .Select(c => Guid.TryParse(c.Value, out var id) ? id : (Guid?)null)
            .Where(id => id.HasValue)
            .Select(id => id!.Value)
            .ToImmutableArray();

        var result = await _banSubThreadUserUseCase.ExecuteAsync(new BanSubThreadUserRequest(
            targetUserId,
            subThreadName,
            requestingUserId,
            moderatedSubThreadIds,
            apiRequest.Reason,
            apiRequest.ExpiresAt
        ));

        if (!result.IsSuccess)
            return result.ErrorType switch
            {
                ErrorType.UserNotFound or ErrorType.SubThreadNotFound => NotFound(new ErrorResponse(result.Message)),
                ErrorType.Forbidden => StatusCode(403, new ErrorResponse(result.Message)),
                ErrorType.BanReasonEmpty or ErrorType.BanReasonTooLong or ErrorType.InvalidBanDate
                    => BadRequest(new ErrorResponse(result.Message)),
                ErrorType.ConcurrencyFailure => Conflict(new ErrorResponse(result.Message)),
                ErrorType.DatabaseTimeout => StatusCode(504, new ErrorResponse(result.Message)),
                _ => StatusCode(500, new ErrorResponse(result.Message))
            };

        return NoContent();
    }

    [HttpDelete("subthreads/{subThreadName}/users/{targetUserId:guid}/ban")]
    [Authorize(Policy = "NotBanned")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status504GatewayTimeout)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> UnbanSubThreadUser(
        [FromRoute] string subThreadName,
        [FromRoute] Guid targetUserId)
    {
        var requestingUserId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var moderatedSubThreadIds = User.FindAll("moderator")
            .Select(c => Guid.TryParse(c.Value, out var id) ? id : (Guid?)null)
            .Where(id => id.HasValue)
            .Select(id => id!.Value)
            .ToImmutableArray();

        var result = await _unbanSubThreadUserUseCase.ExecuteAsync(new UnbanSubThreadUserRequest(
            targetUserId,
            subThreadName,
            requestingUserId,
            moderatedSubThreadIds
        ));

        if (!result.IsSuccess)
            return result.ErrorType switch
            {
                ErrorType.UserNotFound or ErrorType.SubThreadNotFound or ErrorType.NotFound
                    => NotFound(new ErrorResponse(result.Message)),
                ErrorType.Forbidden => StatusCode(403, new ErrorResponse(result.Message)),
                ErrorType.ConcurrencyFailure => Conflict(new ErrorResponse(result.Message)),
                ErrorType.DatabaseTimeout => StatusCode(504, new ErrorResponse(result.Message)),
                _ => StatusCode(500, new ErrorResponse(result.Message))
            };

        return NoContent();
    }

    [HttpPost("subthreads/{subThreadName}/moderators/{targetUserId:guid}")]
    [Authorize(Policy = "NotBanned")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status504GatewayTimeout)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> AssignModerator(
        [FromRoute] string subThreadName,
        [FromRoute] Guid targetUserId)
    {
        var requestingUserId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var result = await _assignModeratorUseCase.ExecuteAsync(new AssignModeratorRequest(
            subThreadName,
            targetUserId,
            requestingUserId
        ));

        if (!result.IsSuccess)
            return result.ErrorType switch
            {
                ErrorType.SubThreadNotFound or ErrorType.UserNotFound => NotFound(new ErrorResponse(result.Message)),
                ErrorType.Forbidden => StatusCode(403, new ErrorResponse(result.Message)),
                ErrorType.AlreadyAssigned or ErrorType.ConcurrencyFailure =>
                    Conflict(new ErrorResponse(result.Message)),
                ErrorType.DatabaseTimeout => StatusCode(504, new ErrorResponse(result.Message)),
                _ => StatusCode(500, new ErrorResponse(result.Message))
            };

        return NoContent();
    }

    [HttpDelete("subthreads/{subThreadName}/moderators/{targetUserId:guid}")]
    [Authorize(Policy = "NotBanned")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status504GatewayTimeout)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> RemoveModerator(
        [FromRoute] string subThreadName,
        [FromRoute] Guid targetUserId)
    {
        var requestingUserId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var result = await _removeModeratorUseCase.ExecuteAsync(new RemoveModeratorRequest(
            subThreadName,
            targetUserId,
            requestingUserId
        ));

        if (!result.IsSuccess)
            return result.ErrorType switch
            {
                ErrorType.SubThreadNotFound or ErrorType.UserNotFound or ErrorType.NotFound
                    => NotFound(new ErrorResponse(result.Message)),
                ErrorType.Forbidden => StatusCode(403, new ErrorResponse(result.Message)),
                ErrorType.ConcurrencyFailure => Conflict(new ErrorResponse(result.Message)),
                ErrorType.DatabaseTimeout => StatusCode(504, new ErrorResponse(result.Message)),
                _ => StatusCode(500, new ErrorResponse(result.Message))
            };

        return NoContent();
    }

    [HttpPost("subthreads/{subThreadName}/reports")]
    [Authorize(Policy = "NotBanned")]
    [ProducesResponseType(typeof(CreateReportApiResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status504GatewayTimeout)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<CreateReportApiResponse>> CreateSubThreadReport(
        [FromRoute] string subThreadName,
        [FromBody] CreateSubThreadReportApiRequest apiRequest)
    {
        if (!TryParseReportType(apiRequest.Type, out var reportType))
            return BadRequest(new ErrorResponse($"Invalid report type '{apiRequest.Type}'."));

        if (!TryParseReportCategory(apiRequest.Category, out var reportCategory))
            return BadRequest(new ErrorResponse($"Invalid report category '{apiRequest.Category}'."));

        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var result = await _createSubThreadReportUseCase
            .ExecuteAsync(new CreateSubThreadReportRequest(
                subThreadName,
                userId,
                reportType,
                apiRequest.TargetId,
                reportCategory,
                apiRequest.Message
            ));

        if (!result.IsSuccess)
            return result.ErrorType switch
            {
                ErrorType.SubThreadNotFound or ErrorType.PostNotFound or ErrorType.CommentNotFound
                    => NotFound(new ErrorResponse(result.Message)),
                ErrorType.PostDoesNotBelongToSubThread or ErrorType.CommentDoesNotBelongToPost
                    or ErrorType.ContentTooLong
                    => BadRequest(new ErrorResponse(result.Message)),
                ErrorType.UserNotFound => Unauthorized(new ErrorResponse(result.Message)),
                ErrorType.DatabaseTimeout => StatusCode(504, new ErrorResponse(result.Message)),
                _ => StatusCode(500, new ErrorResponse(result.Message))
            };

        return StatusCode(201, new CreateReportApiResponse(result.ReportId!.Value));
    }

    [HttpGet("subthreads/{subThreadName}/reports")]
    [Authorize(Policy = "NotBanned")]
    [ProducesResponseType(typeof(GetReportsApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status504GatewayTimeout)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<GetReportsApiResponse>> GetSubThreadReports(
        [FromRoute] string subThreadName,
        [FromQuery] string? status = null,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        Report.ReportStatus? parsedStatus = null;
        if (status is not null)
        {
            if (!TryParseReportStatus(status, out var reportStatus))
                return BadRequest(new ErrorResponse($"Invalid status filter '{status}'."));
            parsedStatus = reportStatus;
        }

        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var isSiteAdmin = User.HasClaim("role", "SiteAdmin");
        var isSiteOwner = User.HasClaim("role", "SiteOwner");
        var moderatedSubThreadIds = User.FindAll("moderator")
            .Select(c => Guid.TryParse(c.Value, out var id) ? id : (Guid?)null)
            .Where(id => id.HasValue).Select(id => id!.Value).ToImmutableArray();

        var result = await _getSubThreadReportsUseCase.ExecuteAsync(new GetSubThreadReportsRequest(
            subThreadName,
            userId,
            moderatedSubThreadIds,
            isSiteAdmin,
            isSiteOwner,
            parsedStatus,
            page,
            pageSize
        ));

        if (!result.IsSuccess)
            return result.ErrorType switch
            {
                ErrorType.SubThreadNotFound => NotFound(new ErrorResponse(result.Message)),
                ErrorType.Forbidden => StatusCode(403, new ErrorResponse(result.Message)),
                ErrorType.DatabaseTimeout => StatusCode(504, new ErrorResponse(result.Message)),
                _ => StatusCode(500, new ErrorResponse(result.Message))
            };

        return Ok(new GetReportsApiResponse(
            result.Reports.Select(r => new ReportApiDto(
                r.Id, r.Type.ToString(), r.Category.ToString(), r.Status.ToString(), r.Message,
                r.ReporterId, r.ReporterUsername,
                r.SubThreadId, r.SubThreadName,
                r.TargetPostId, r.TargetPostTitle,
                r.TargetCommentId, r.TargetCommentContent,
                r.TargetUserId, r.TargetUsername,
                r.TargetSubThreadId, r.TargetSubThreadName,
                r.TargetDirectMessageId, r.TargetDirectMessageContent,
                r.ReportedAt)).ToImmutableList()
        ));
    }

    [HttpPatch("subthreads/{subThreadName}/reports/{reportId:guid}/status")]
    [Authorize(Policy = "NotBanned")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status504GatewayTimeout)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> SetSubThreadReportStatus(
        [FromRoute] string subThreadName,
        [FromRoute] Guid reportId,
        [FromBody] SetReportStatusApiRequest apiRequest)
    {
        if (!TryParseReportStatus(apiRequest.NewStatus, out var newStatus))
            return BadRequest(new ErrorResponse($"Invalid status '{apiRequest.NewStatus}'."));

        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var isSiteAdmin = User.HasClaim("role", "SiteAdmin");
        var isSiteOwner = User.HasClaim("role", "SiteOwner");
        var moderatedSubThreadIds = User.FindAll("moderator")
            .Select(c => Guid.TryParse(c.Value, out var id) ? id : (Guid?)null)
            .Where(id => id.HasValue).Select(id => id!.Value).ToImmutableArray();

        var result = await _setReportStatusUseCase.ExecuteAsync(new SetReportStatusRequest(
            reportId,
            userId,
            isSiteAdmin,
            isSiteOwner,
            moderatedSubThreadIds,
            newStatus
        ));

        if (!result.IsSuccess)
            return result.ErrorType switch
            {
                ErrorType.ReportNotFound or ErrorType.SubThreadNotFound => NotFound(new ErrorResponse(result.Message)),
                ErrorType.Forbidden => StatusCode(403, new ErrorResponse(result.Message)),
                ErrorType.SameStatus => BadRequest(new ErrorResponse(result.Message)),
                ErrorType.ConcurrencyFailure => Conflict(new ErrorResponse(result.Message)),
                ErrorType.DatabaseTimeout => StatusCode(504, new ErrorResponse(result.Message)),
                _ => StatusCode(500, new ErrorResponse(result.Message))
            };

        return NoContent();
    }

    private static bool TryParseReportType(string value, out Report.ReportType result)
        => Enum.TryParse(value, ignoreCase: true, out result);

    private static bool TryParseReportCategory(string value, out Report.ReportCategory result)
        => Enum.TryParse(value, ignoreCase: true, out result);

    private static bool TryParseReportStatus(string value, out Report.ReportStatus result)
        => Enum.TryParse(value, ignoreCase: true, out result);
}