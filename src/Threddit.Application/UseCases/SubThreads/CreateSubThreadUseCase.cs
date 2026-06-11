using Microsoft.Extensions.Logging;
using Threddit.Application.DTOs.Common;
using Threddit.Application.DTOs.Requests.SubThreads;
using Threddit.Application.DTOs.Responses.SubThreads;
using Threddit.Application.Interfaces.Driven;
using Threddit.Application.Interfaces.Driving;
using Threddit.Domain.Common;
using Threddit.Domain.Entities;

namespace Threddit.Application.UseCases.SubThreads;

public sealed partial class CreateSubThreadUseCase : ICreateSubThreadUseCase
{
    private readonly ISubThreadRepository _subThreadRepository;
    private readonly IUserRepository _userRepository;
    private readonly ILogger<CreateSubThreadUseCase> _logger;

    public CreateSubThreadUseCase(
        ISubThreadRepository subThreadRepository,
        IUserRepository userRepository,
        ILogger<CreateSubThreadUseCase> logger
    )
    {
        _subThreadRepository = subThreadRepository;
        _userRepository = userRepository;
        _logger = logger;
    }

    public async Task<CreateSubThreadResponse> ExecuteAsync(CreateSubThreadRequest request)
    {
        var userResult = await _userRepository.GetByIdAsync(request.RequestingUserId);
        if (!userResult.IsSuccess)
        {
            LogUserNotFound(request.RequestingUserId, userResult.ErrorMessage);
            var message = ResolveErrorMessage(userResult.ErrorType);
            return new CreateSubThreadResponse(false,
                Message: message, ErrorType: userResult.ErrorType);
        }
        var user = userResult.Value!;
        
        var existingResult = await _subThreadRepository.GetByNameAsync(request.Name);
        if (existingResult.IsSuccess)
        {
            LogSubThreadAlreadyExists(user.Id, request.Name);
            var message = ResolveErrorMessage(ErrorType.AlreadyAssigned);
            return new CreateSubThreadResponse(false,
                Message: message, ErrorType: ErrorType.AlreadyAssigned);
        }
        
        var createResult = SubThread.Create(request.Name, user,
            request.Description, request.BannerImageUrl);
        
        if (!createResult.IsSuccess)
        {
            LogCreationFailure(createResult.Exception, request.Name, createResult.ErrorMessage);
            var message = ResolveErrorMessage(createResult.ErrorType);
            return new CreateSubThreadResponse(false,
                Message: message, ErrorType: createResult.ErrorType);
        }

        var saveResult = await _subThreadRepository.CreateAsync(createResult.Value!);
        if (!saveResult.IsSuccess)
        {
            LogCreationFailure(saveResult.Exception, request.Name, saveResult.ErrorMessage);
            var message = ResolveErrorMessage(saveResult.ErrorType);
            return new CreateSubThreadResponse(false,
                Message: message, ErrorType: saveResult.ErrorType);
        }
        
        var subThread = saveResult.Value!;
        
        var dto = new SubThreadDto(
            subThread.Id,
            subThread.CreatedBy?.UserName,
            subThread.Name,
            subThread.Description,
            subThread.BannerImageUrl,
            subThread.SubscriberCount,
            subThread.CreatedAt,
            null,
            []
        );
        
        LogCreationSuccess(subThread.Name, subThread.Id);
        return new CreateSubThreadResponse(true, dto);
    }

    private static string ResolveErrorMessage(ErrorType errorType) => errorType switch
    {
        ErrorType.UserNotFound => "User not found.",
        ErrorType.NameEmpty => "SubThread name cannot be empty.",
        ErrorType.NameTooLong => $"SubThread name cannot exceed {SubThread.Limits.MaxNameLength} characters.",
        ErrorType.ContentTooLong =>
            $"SubThread description cannot exceed {SubThread.Limits.MaxDescriptionLength} characters.",
        ErrorType.ImageUrlTooLong =>
            $"SubThread banner image URL cannot exceed {SubThread.Limits.MaxBannerImageUrlLength} characters.",
        ErrorType.AlreadyAssigned => "A SubThread with that name already exists.",
        ErrorType.ConcurrencyFailure => "SubThread creation failed due to internal server error. Please refresh and try again.",
        ErrorType.DatabaseTimeout => "The request timed out. Please try again later.",
        _ => "An unexpected error occurred trying to create SubThread. Please try again later."
    };
}