using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Threddit.Application.DTOs.Requests.Auth;
using Threddit.Application.Interfaces.Driving;
using Threddit.Contracts.Common;
using Threddit.Contracts.Requests.Auth;
using Threddit.Contracts.Responses.Auth;
using Threddit.Domain.Common;

namespace Threddit.API.Controllers;

[ApiController]
[Route("api/auth")]
public sealed class AuthController : ControllerBase
{
    private readonly IRegistrationUseCase _registrationUseCase;
    private readonly ILoginUseCase _loginUseCase;

    public AuthController(
        IRegistrationUseCase registrationUseCase,
        ILoginUseCase loginUseCase
    )
    {
        _registrationUseCase = registrationUseCase;
        _loginUseCase = loginUseCase;
    }

    [HttpPost("register")]
    [EnableRateLimiting("registration")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Register([FromBody] RegistrationApiRequest apiRequest)
    {
        var request = new RegistrationRequest(
            apiRequest.Username,
            apiRequest.Email,
            apiRequest.Password
        );

        var result = await _registrationUseCase.ExecuteAsync(request);

        if (!result.IsSuccess)
        {
            return result.ErrorType switch
            {
                ErrorType.EmailTaken or ErrorType.UsernameTaken => Conflict(new ErrorResponse(result.Message)),
                _ => BadRequest(new ErrorResponse(result.Message))
            };
        }

        return Created();
    }

    [HttpPost("login")]
    [EnableRateLimiting("login")]
    [ProducesResponseType(typeof(LoginApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]   
    public async Task<ActionResult<LoginApiResponse>> Login([FromBody] LoginApiRequest apiRequest)
    {
        var request = new LoginRequest(
            apiRequest.UsernameOrEmail,
            apiRequest.Password
        );

        var result = await _loginUseCase.ExecuteAsync(request);

        if (!result.IsSuccess)
        {
            return result.ErrorType switch
            {
                ErrorType.InvalidCredentials => Unauthorized(new ErrorResponse(result.Message)),
                _ => BadRequest(new ErrorResponse(result.Message))
            };
        }

        return Ok(new LoginApiResponse(
            result.JwtToken!,
            result.ExpiresAt!.Value,
            result.Username!
        ));
    }

    [HttpPost("logout")]
    [Authorize(Policy = "NotBanned")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]  
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public IActionResult Logout()
    {
        return NoContent();
    }
}