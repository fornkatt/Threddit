using System.Collections.Immutable;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Threddit.Application.DTOs.Requests.Users;
using Threddit.Application.Interfaces.Driving;
using Threddit.Contracts.Common;
using Threddit.Contracts.Requests.Users;
using Threddit.Contracts.Responses.Users;
using Threddit.Domain.Common;

namespace Threddit.API.Controllers;

[ApiController]
[Route("api/users")]
public class UserController : ControllerBase
{
    private readonly IGetUserProfileUseCase _getUserProfileUseCase;
    private readonly IGetCurrentUserSummaryUseCase _getCurrentUserSummaryUseCase;
    private readonly IEditUserProfileUseCase _editUserProfileUseCase;

    public UserController(
        IGetUserProfileUseCase getUserProfileUseCase,
        IGetCurrentUserSummaryUseCase getCurrentUserSummaryUseCase,
        IEditUserProfileUseCase editUserProfileUseCase
    )
    {
        _getUserProfileUseCase = getUserProfileUseCase;
        _getCurrentUserSummaryUseCase = getCurrentUserSummaryUseCase;
        _editUserProfileUseCase = editUserProfileUseCase;
    }

    [HttpGet("{username}")]
    [ProducesResponseType(typeof(GetUserProfileApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status504GatewayTimeout)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<GetUserProfileApiResponse>> GetProfile(
        [FromRoute] string username)
    {
        var result = await _getUserProfileUseCase.ExecuteAsync(new GetUserProfileRequest(username));
        if (!result.IsSuccess)
            return result.ErrorType switch
            {
                ErrorType.UserNotFound => NotFound(new ErrorResponse(result.Message)),
                ErrorType.DatabaseTimeout => StatusCode(504, new ErrorResponse(result.Message)),
                _ => StatusCode(500, new ErrorResponse(result.Message))
            };

        var user = result.User!;

        return Ok(new GetUserProfileApiResponse(
            user.Id,
            user.Username,
            user.ProfilePicture,
            user.Description,
            user.PostScore,
            user.CommentScore,
            user.TotalScore,
            user.CreationDate
        ));
    }

    [HttpGet("me/summary")]
    [Authorize(Policy = "NotBanned")]
    [ProducesResponseType(typeof(GetCurrentUserSummaryApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status504GatewayTimeout)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<GetCurrentUserSummaryApiResponse>> GetCurrentUserSummary()
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var result = await _getCurrentUserSummaryUseCase
            .ExecuteAsync(new GetCurrentUserSummaryRequest(userId));

        if (!result.IsSuccess)
            return result.ErrorType switch
            {
                ErrorType.UserNotFound => NotFound(new ErrorResponse(result.Message)),
                ErrorType.DatabaseTimeout => StatusCode(504, new ErrorResponse(result.Message)),
                _ => StatusCode(500, new ErrorResponse(result.Message))
            };

        var summary = result.Summary!;

        return Ok(new GetCurrentUserSummaryApiResponse(
            summary.UserId,
            summary.Username,
            summary.IsSiteOwner,
            summary.IsSiteAdmin,
            summary.Subscriptions.Select(s => new SubscribedSubThreadApiDto(s.Id, s.Name))
                .ToList().AsReadOnly(),
            summary.OwnedSubThreadIds,
            summary.ModeratedSubThreadIds
        ));
    }

    [HttpPatch("me")]
    [Authorize(Policy = "NotBanned")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status504GatewayTimeout)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> EditProfile(
        [FromBody] EditUserProfileApiRequest apiRequest)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var result = await _editUserProfileUseCase.ExecuteAsync(new EditUserProfileRequest(
            userId,
            apiRequest.ProfilePicture,
            apiRequest.Description
        ));

        if (!result.IsSuccess)
            return result.ErrorType switch
            {
                ErrorType.UserNotFound => NotFound(new ErrorResponse(result.Message)),
                ErrorType.ImageUrlTooLong or ErrorType.ContentTooLong
                    => BadRequest(new ErrorResponse(result.Message)),
                ErrorType.ConcurrencyFailure => Conflict(new ErrorResponse(result.Message)),
                ErrorType.DatabaseTimeout => StatusCode(504, new ErrorResponse(result.Message)),
                _ => StatusCode(500, new ErrorResponse(result.Message))
            };

        return NoContent();
    }
}