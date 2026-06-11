using Microsoft.AspNetCore.Mvc;
using Threddit.Application.Interfaces.Driving;
using Threddit.Contracts.Responses.Validation;

namespace Threddit.API.Controllers;

[ApiController]
[Route("api/validation")]
public sealed class ValidationController : ControllerBase
{
    private readonly IGetValidationLimitsUseCase _getValidationLimitsUseCase;

    public ValidationController(
        IGetValidationLimitsUseCase getValidationLimitsUseCase
    )
    {
        _getValidationLimitsUseCase = getValidationLimitsUseCase;
    }

    [HttpGet("limits")]
    [ProducesResponseType(typeof(ValidationLimitsApiResponse), StatusCodes.Status200OK)]
    public ActionResult<ValidationLimitsApiResponse> GetLimits()
    {
        var limits = _getValidationLimitsUseCase.Execute();

        return Ok(new ValidationLimitsApiResponse(
            limits.PostMaxTitleLength,
            limits.PostMaxSlugLength,
            limits.PostMaxContentLength,
            limits.PostMaxImageUrlLength,
            limits.PostMaxDeleteReasonLength,
            
            limits.CommentMaxContentLength,
            limits.CommentMaxDeleteReasonLength,
            
            limits.SubThreadMaxNameLength,
            limits.SubThreadMaxDescriptionLength,
            limits.SubThreadMaxBannerImageUrlLength,
            
            limits.SubThreadRuleMaxTitleLength,
            limits.SubThreadRuleMaxContentLength,
            
            limits.UserMaxUsernameLength,
            limits.UserMaxEmailLength,
            limits.UserMaxProfilePictureUrlLength,
            limits.UserMaxDescriptionLength,
            
            limits.GroupConversationMaxNameLength,
            
            limits.DirectMessageMaxContentLength,
            
            limits.ReportMaxMessageLength
        ));
    }
}