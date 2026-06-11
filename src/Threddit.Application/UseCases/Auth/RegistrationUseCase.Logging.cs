using Microsoft.Extensions.Logging;

namespace Threddit.Application.UseCases.Auth;

public sealed partial class RegistrationUseCase
{
    [LoggerMessage(
        Level = LogLevel.Warning,
        Message = "Registration attempt with already taken email: {Email}")]
    private partial void LogEmailTaken(string email);
    
    [LoggerMessage(
        Level = LogLevel.Warning,
        Message = "Registration attempt with already taken username: {Username}")]
    private partial void LogUsernameTaken(string username);

    [LoggerMessage(
        Level = LogLevel.Error,
        Message = "User creation failed during registration: {ErrorMessage}")]
    private partial void LogUserCreationFailed(Exception? ex, string? errorMessage);
    
    [LoggerMessage(
        Level = LogLevel.Error,
        Message = "Registration failed for {Username}: {Errors}")]
    private partial void LogRegistrationFailed(string? username, string errors);
    
    [LoggerMessage(
        Level = LogLevel.Information,
        Message = "User registered successfully: {Username} ({UserId})")]
    private partial void LogUserRegistered(string username, Guid userId);
}