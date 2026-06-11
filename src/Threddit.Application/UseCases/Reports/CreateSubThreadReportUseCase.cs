using Microsoft.Extensions.Logging;
using Threddit.Application.DTOs.Requests.Reports;
using Threddit.Application.DTOs.Responses.Reports;
using Threddit.Application.Interfaces.Driven;
using Threddit.Application.Interfaces.Driving;
using Threddit.Domain.Common;
using Threddit.Domain.Entities;

namespace Threddit.Application.UseCases.Reports;

public sealed partial class CreateSubThreadReportUseCase : ICreateSubThreadReportUseCase
{
    private readonly IReportRepository _reportRepository;
    private readonly ISubThreadRepository _subThreadRepository;
    private readonly IUserRepository _userRepository;
    private readonly IPostRepository _postRepository;
    private readonly ICommentRepository _commentRepository;
    private readonly ILogger<CreateSubThreadReportUseCase> _logger;

    public CreateSubThreadReportUseCase(
        IReportRepository reportRepository,
        ISubThreadRepository subThreadRepository,
        IUserRepository userRepository,
        IPostRepository postRepository,
        ICommentRepository commentRepository,
        ILogger<CreateSubThreadReportUseCase> logger
    )
    {
        _reportRepository = reportRepository;
        _subThreadRepository = subThreadRepository;
        _userRepository = userRepository;
        _postRepository = postRepository;
        _commentRepository = commentRepository;
        _logger = logger;
    }

    public async Task<CreateReportResponse> ExecuteAsync(CreateSubThreadReportRequest request)
    {
        var subThreadResult = await _subThreadRepository.GetByNameAsync(request.SubThreadName);
        if (!subThreadResult.IsSuccess)
        {
            LogSubThreadFetchFailure(subThreadResult.Exception, request.SubThreadName, subThreadResult.ErrorMessage);
            var message = ResolveErrorMessage(subThreadResult.ErrorType);
            return new CreateReportResponse(false, message, subThreadResult.ErrorType);
        }

        var reporterResult = await _userRepository.GetByIdAsync(request.RequestingUserId);
        if (!reporterResult.IsSuccess)
        {
            LogUserFetchFailure(reporterResult.Exception, request.RequestingUserId, reporterResult.ErrorMessage);
            var message = ResolveErrorMessage(reporterResult.ErrorType);
            return new CreateReportResponse(false, message, reporterResult.ErrorType);
        }

        var subThread = subThreadResult.Value!;
        var reporter = reporterResult.Value!;

        Result<Report> createResult;

        switch (request.Type)
        {
            case Report.ReportType.Post:
            {
                var postResult = await _postRepository.GetByIdAsync(request.TargetId);
                if (!postResult.IsSuccess)
                {
                    LogPostFetchFailure(postResult.Exception, request.TargetId, postResult.ErrorMessage);
                    var message = ResolveErrorMessage(postResult.ErrorType);
                    return new CreateReportResponse(false, message, postResult.ErrorType);
                }
                var post = postResult.Value!;
            
                createResult = Report.ForPost(reporter, post, subThread.Id, request.Category, request.Message);
                break;
            }
            case Report.ReportType.Comment:
            {
                var commentResult = await _commentRepository.GetByIdAsync(request.TargetId);
                if (!commentResult.IsSuccess)
                {
                    LogCommentFetchFailure(commentResult.Exception, request.TargetId, commentResult.ErrorMessage);
                    var message = ResolveErrorMessage(commentResult.ErrorType);
                    return new CreateReportResponse(false, message, commentResult.ErrorType);
                }
                var comment = commentResult.Value!;
            
                createResult = Report.ForComment(reporter, comment, comment.PostId, subThread.Id,
                    request.Category, request.Message);
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

        var saveResult = await _reportRepository.CreateAsync(createResult.Value!);
        if (!saveResult.IsSuccess)
        {
            LogSaveFailure(saveResult.Exception, saveResult.ErrorMessage);
            var message = ResolveErrorMessage(saveResult.ErrorType);
            return new CreateReportResponse(false, message, saveResult.ErrorType);
        }
        var savedReport = saveResult.Value!;
        
        LogCreateSuccess(savedReport.Id, request.Type.ToString());
        return new CreateReportResponse(true, "Report submitted successfully.",
            ReportId: savedReport.Id);
    }
    
    private static string ResolveErrorMessage(ErrorType errorType) => errorType switch
    {
        ErrorType.SubThreadNotFound => "SubThread not found.",
        ErrorType.UserNotFound => "User not found.",
        ErrorType.PostNotFound => "Post not found.",
        ErrorType.CommentNotFound => "Comment not found.",
        ErrorType.InvalidReportType => "Invalid report type specified for a SubThread report.",
        ErrorType.PostDoesNotBelongToSubThread => "The post does not belong to this SubThread.",
        ErrorType.CommentDoesNotBelongToPost => "The comment does not belong to the specified post.",
        ErrorType.ContentTooLong => $"Report message cannot exceed {Report.Limits.MaxMessageLength} characters.",
        ErrorType.DatabaseTimeout => "The request timed out. Please try again later.",
        _ => "An unexpected error occurred. Please try again later."
    };
}