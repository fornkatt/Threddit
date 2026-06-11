using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Threddit.Application.DTOs.Requests.Auth;
using Threddit.Application.DTOs.Responses.Auth;
using Threddit.Application.Interfaces.Driven;
using Threddit.Application.Interfaces.Driving;
using Threddit.Domain.Common;
using Threddit.Domain.Entities;

namespace Threddit.Application.UseCases.Auth;

public sealed partial class LoginUseCase : ILoginUseCase
{
    private readonly UserManager<User> _userManager;
    private readonly IJwtService _jwtService;
    private readonly IUserRepository _userRepository;
    private readonly ILogger<LoginUseCase> _logger;

    public LoginUseCase(
        UserManager<User> userManager,
        IJwtService jwtService,
        IUserRepository userRepository,
        ILogger<LoginUseCase> logger
    )
    {
        _userManager = userManager;
        _jwtService = jwtService;
        _userRepository = userRepository;
        _logger = logger;
    }

    public async Task<LoginResponse> ExecuteAsync(LoginRequest request)
    {
        var user = request.UsernameOrEmail.Contains('@')
            ? await _userManager.FindByEmailAsync(request.UsernameOrEmail)
            : await _userManager.FindByNameAsync(request.UsernameOrEmail);

        if (user is null)
        {
            LogUnknownUserLoginAttempt(request.UsernameOrEmail);
            return new LoginResponse(false, Message: "Wrong username or password.",
                ErrorType: ErrorType.InvalidCredentials);
        }

        var isPasswordValid = await _userManager.CheckPasswordAsync(user, request.Password);
        if (!isPasswordValid)
        {
            LogInvalidPasswordLoginAttempt(user.Id);
            return new LoginResponse(false, Message: "Wrong username or password.",
                ErrorType: ErrorType.InvalidCredentials);
        }

        var userWithRoles = await _userRepository.GetByIdWithRolesAsync(user.Id);
        if (!userWithRoles.IsSuccess)
        {
            LogRoleFetchFailure(userWithRoles.Exception, user.Id, userWithRoles.ErrorMessage);
            return new LoginResponse(false, Message: "Login failed. Please try again.",
                ErrorType: ErrorType.Unknown);
        }

        var (token, expiresAt) = _jwtService.GenerateToken(userWithRoles.Value!);

        LogSuccessfulLogin(user.UserName!, user.Id);
        return new LoginResponse(true, token, expiresAt, user.UserName);
    }
}