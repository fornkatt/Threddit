using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Threddit.Application.DTOs.Requests;
using Threddit.Application.DTOs.Requests.Auth;
using Threddit.Application.DTOs.Responses;
using Threddit.Application.DTOs.Responses.Auth;
using Threddit.Application.Interfaces.Driving;
using Threddit.Domain.Common;
using Threddit.Domain.Entities;

namespace Threddit.Application.UseCases.Auth;

public sealed partial class RegistrationUseCase : IRegistrationUseCase
{
    private readonly UserManager<User> _userManager;
    private readonly ILogger<RegistrationUseCase> _logger;

    public RegistrationUseCase(
        UserManager<User> userManager,
        ILogger<RegistrationUseCase> logger
    )
    {
        _userManager = userManager;
        _logger = logger;
    }

    public async Task<RegistrationResponse> ExecuteAsync(RegistrationRequest request)
    {
        var emailTaken = await _userManager.FindByEmailAsync(request.Email);
        if (emailTaken is not null)
        {
            LogEmailTaken(request.Email);
            var message = ResolveErrorMessage(ErrorType.EmailTaken);
            return new RegistrationResponse(false, message, ErrorType.EmailTaken);
        }

        var usernameTaken = await _userManager.FindByNameAsync(request.Username);
        if (usernameTaken is not null)
        {
            LogUsernameTaken(request.Username);
            var message = ResolveErrorMessage(ErrorType.UsernameTaken);
            return new RegistrationResponse(false, message, ErrorType.UsernameTaken);
        }

        var userResult = User.Create(request.Username, request.Email);
        if (!userResult.IsSuccess)
        {
            var userMessage = ResolveErrorMessage(userResult.ErrorType);
            LogUserCreationFailed(userResult.Exception, userResult.ErrorMessage);
            return new RegistrationResponse(false, userMessage, userResult.ErrorType);
        }

        var result = await _userManager.CreateAsync(userResult.Value!, request.Password);
        if (!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            LogRegistrationFailed(request.Username, errors);
            return new RegistrationResponse(false, errors, ErrorType.RegistrationFailed);
        }

        LogUserRegistered(userResult.Value!.UserName!, userResult.Value.Id);
        return new RegistrationResponse(true, "Registration successful.");
    }

    private static string ResolveErrorMessage(ErrorType errorType)
    {
        return errorType switch
        {
            ErrorType.EmailTaken => "Email already in use.",
            ErrorType.UsernameTaken => "Username already in use.",
            ErrorType.InvalidUsername => "Username cannot be empty.",
            ErrorType.InvalidEmail => "Email cannot be empty.",
            ErrorType.UsernameTooLong => $"Username cannot exceed {User.Limits.MaxUsernameLength} characters.",
            ErrorType.EmailTooLong => $"Email cannot exceed {User.Limits.MaxEmailLength} characters.",
            _ => "Unknown error during registration."
        };
    }
}