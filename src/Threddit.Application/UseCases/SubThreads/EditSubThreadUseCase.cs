using Microsoft.Extensions.Logging;
using Threddit.Application.DTOs.Requests.SubThreads;
using Threddit.Application.DTOs.Responses.SubThreads;
using Threddit.Application.Interfaces.Driven;
using Threddit.Application.Interfaces.Driving;
using Threddit.Domain.Common;
using Threddit.Domain.Entities;

namespace Threddit.Application.UseCases.SubThreads;

public sealed partial class EditSubThreadUseCase : IEditSubThreadUseCase
{
    private readonly ISubThreadRepository _subThreadRepository;
    private readonly ILogger<EditSubThreadUseCase> _logger;

    public EditSubThreadUseCase(
        ISubThreadRepository subThreadRepository,
        ILogger<EditSubThreadUseCase> logger
    )
    {
        _subThreadRepository = subThreadRepository;
        _logger = logger;
    }

    public async Task<EditSubThreadResponse> ExecuteAsync(EditSubThreadRequest request)
    {
        var fetchResult = await _subThreadRepository.GetByNameAsync(request.SubThreadName);
        if (!fetchResult.IsSuccess)
        {
            LogFetchFailure(fetchResult.Exception, request.SubThreadName, fetchResult.ErrorMessage);
            var message = ResolveErrorMessage(fetchResult.ErrorType);
            return new EditSubThreadResponse(false, message, fetchResult.ErrorType);
        }
        
        var subThread = fetchResult.Value!;
        
        var isSubThreadOwner = subThread.SubThreadOwner.UserId == request.RequestingUserId;
        
        if (!isSubThreadOwner)
        {
            LogUnauthorizedEditAttempt(request.RequestingUserId, subThread.Id);
            var message = ResolveErrorMessage(ErrorType.Forbidden);
            return new EditSubThreadResponse(false, message, ErrorType.Forbidden);
        }

        var editResult = subThread.Edit(request.Description, request.BannerImageUrl);
        if (!editResult.IsSuccess)
        {
            LogValidationFailure(subThread.Id, editResult.ErrorMessage);
            var message = ResolveErrorMessage(editResult.ErrorType);
            return new EditSubThreadResponse(false, message, editResult.ErrorType);
        }

        var updateResult = await _subThreadRepository.UpdateAsync(subThread);
        if (!updateResult.IsSuccess)
        {
            LogUpdateFailure(updateResult.Exception, subThread.Id, updateResult.ErrorMessage);
            var message = ResolveErrorMessage(updateResult.ErrorType);
            return new EditSubThreadResponse(false, message, updateResult.ErrorType);
        }
        
        LogEditSuccess(subThread.Name, subThread.Id, request.RequestingUserId);
        return new EditSubThreadResponse(true, "SubThread updated successfully.");
    }

    private static string ResolveErrorMessage(ErrorType errorType) => errorType switch
    {
        ErrorType.SubThreadNotFound => "SubThread not found.",
        ErrorType.Forbidden => "You are not authorized to edit this SubThread.",
        ErrorType.ContentTooLong => $"Description cannot exceed {SubThread.Limits.MaxDescriptionLength} characters.",
        ErrorType.ImageUrlTooLong => $"Banner image URL cannot exceed {SubThread.Limits.MaxBannerImageUrlLength} characters.",
        ErrorType.ConcurrencyFailure => "The SubThread may have been modified by another user. Please refresh and try again.",
        ErrorType.DatabaseTimeout => "The request timed out. Please try again later.",
        _ => "An unexpected error occurred. Please try again later."
    };
}