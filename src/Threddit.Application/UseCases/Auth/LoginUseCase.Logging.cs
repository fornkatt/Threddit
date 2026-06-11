using Microsoft.Extensions.Logging;

namespace Threddit.Application.UseCases.Auth;

public sealed partial class LoginUseCase
{
    [LoggerMessage(
        Level = LogLevel.Warning,
        Message = "Login attempt for unknows user: {UsernameOrEmail}"
        )]
    private partial void LogUnknownUserLoginAttempt(string usernameOrEmail);
    
    [LoggerMessage(
        Level = LogLevel.Warning,
        Message = "Invalid password login attempt for user with ID: {UserId}"
        )]
    private partial void LogInvalidPasswordLoginAttempt(Guid userId);
    
    [LoggerMessage(
        Level = LogLevel.Error,
        Message = "Failed to fetch roles for user with ID: {UserId}. Error: {ErrorMessage}"
        )]
    private partial void LogRoleFetchFailure(Exception? ex, Guid? userId, string? errorMessage);
    
    [LoggerMessage(
        Level = LogLevel.Information,
        Message = "Successful login for user '{Username}' with ID: {UserId}"
        )]
    private partial void LogSuccessfulLogin(string username, Guid userId);
}