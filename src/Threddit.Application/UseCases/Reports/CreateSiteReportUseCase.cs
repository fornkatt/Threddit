using Microsoft.Extensions.Logging;
using Threddit.Application.DTOs.Requests.Reports;
using Threddit.Application.DTOs.Responses.Reports;
using Threddit.Application.Interfaces.Driven;
using Threddit.Application.Interfaces.Driving;
using Threddit.Domain.Common;
using Threddit.Domain.Entities;

namespace Threddit.Application.UseCases.Reports;

public sealed partial class CreateSiteReportUseCase : ICreateSiteReportUseCase
{
    private readonly IReportRepository _reportRepository;
    private readonly IUserRepository _userRepository;
    private readonly ISubThreadRepository _subThreadRepository;
    private readonly IConversationRepository _conversationRepository;
    private readonly ILogger<CreateSiteReportUseCase> _logger;

    public CreateSiteReportUseCase(
        IReportRepository reportRepository,
        IUserRepository userRepository,
        ISubThreadRepository subThreadRepository,
        IConversationRepository conversationRepository,
        ILogger<CreateSiteReportUseCase> logger
    )
    {
        _reportRepository = reportRepository;
        _userRepository = userRepository;
        _subThreadRepository = subThreadRepository;
        _conversationRepository = conversationRepository;
        _logger = logger;
    }

    public async Task<CreateReportResponse> ExecuteAsync(CreateSiteReportRequest request)
    {
        var reporterResult = await _userRepository.GetByIdAsync(request.RequestingUserId);
        if (!reporterResult.IsSuccess)
        {
            LogUserFetchFailure(reporterResult.Exception, request.RequestingUserId, reporterResult.ErrorMessage);
            var message = ResolveErrorMessage(reporterResult.ErrorType);
            return new CreateReportResponse(false, message, reporterResult.ErrorType);
        }

        var reporter = reporterResult.Value!;

        Result<Report> createResult;

        switch (request.Type)
        {
            case Report.ReportType.User:
            {
                var targetResult = await _userRepository.GetByIdAsync(request.TargetId);
                if (!targetResult.IsSuccess)
                {
                    LogTargetFetchFailure(targetResult.Exception, request.TargetId, targetResult.ErrorMessage);
                    var message = ResolveErrorMessage(targetResult.ErrorType);
                    return new CreateReportResponse(false, message, targetResult.ErrorType);
                }

                var targetUser = targetResult.Value!;
                createResult = Report.ForUser(reporter, targetUser, request.Category, request.Message);
                break;
            }
            case Report.ReportType.SubThread:
            {
                var targetResult = await _subThreadRepository.GetByIdAsync(request.TargetId);
                if (!targetResult.IsSuccess)
                {
                    LogTargetFetchFailure(targetResult.Exception, request.TargetId, targetResult.ErrorMessage);
                    var message = ResolveErrorMessage(targetResult.ErrorType);
                    return new CreateReportResponse(false, message, targetResult.ErrorType);
                }

                var targetSubThread = targetResult.Value!;
                createResult = Report.ForSubThread(reporter, targetSubThread, request.Category, request.Message);
                break;
            }
            case Report.ReportType.DirectMessage:
            {
                var targetResult = await _conversationRepository.GetByIdAsync(request.TargetId);
                if (!targetResult.IsSuccess)
                {
                    LogTargetFetchFailure(targetResult.Exception, request.TargetId, targetResult.ErrorMessage);
                    var message = ResolveErrorMessage(targetResult.ErrorType);
                    return new CreateReportResponse(false, message, targetResult.ErrorType);
                }

                var targetDirectMessage = targetResult.Value!;
                createResult =
                    Report.ForDirectMessage(reporter, targetDirectMessage, request.Category, request.Message);
                break;
            }
            default:
            {
                var message = ResolveErrorMessage(ErrorType.InvalidReportType);
                return new CreateReportResponse(false, message, ErrorType.InvalidReportType);
            }
        }

        if (!createResult.IsSuccess)
        {
            LogValidationFailure(createResult.ErrorMessage);
            var message = ResolveErrorMessage(createResult.ErrorType);
            return new CreateReportResponse(false, message, createResult.ErrorType);
        }

        var report = createResult.Value!;

        var saveResult = await _reportRepository.CreateAsync(report);
        if (!saveResult.IsSuccess)
        {
            LogSaveFailure(saveResult.Exception, saveResult.ErrorMessage);
            var message = ResolveErrorMessage(ErrorType.DatabaseUpdateFailure);
            return new CreateReportResponse(false, message, ErrorType.DatabaseUpdateFailure);
        }

        var savedReport = saveResult.Value!;

        LogCreateSuccess(savedReport.Id, request.Type.ToString());
        return new CreateReportResponse(true, "Report submitted successfully.",
            ReportId: savedReport.Id);
    }

    private static string ResolveErrorMessage(ErrorType errorType) => errorType switch
    {
        ErrorType.UserNotFound => "User not found.",
        ErrorType.SubThreadNotFound => "SubThread not found.",
        ErrorType.NotFound => "The reported item was not found.",
        ErrorType.InvalidReportType => "Invalid report type specified for a site report.",
        ErrorType.ContentTooLong => $"Report message cannot exceed {Report.Limits.MaxMessageLength} characters.",
        ErrorType.DatabaseUpdateFailure => "Database error occurred while saving the report.",
        ErrorType.DatabaseTimeout => "The request timed out. Please try again later.",
        _ => "An unexpected error occurred. Please try again later."
    };
}