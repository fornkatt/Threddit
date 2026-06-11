using Microsoft.Extensions.Logging;
using Threddit.Application.DTOs.Requests.Comments;
using Threddit.Application.DTOs.Responses.Comments;
using Threddit.Application.Interfaces.Driven;
using Threddit.Application.Interfaces.Driving;
using Threddit.Application.Mappers;

namespace Threddit.Application.UseCases.Comments;

public sealed partial class GetCommentRepliesUseCase : IGetCommentRepliesUseCase
{
    private readonly ICommentRepository _commentRepository;
    private readonly ILogger<GetCommentRepliesUseCase> _logger;

    public GetCommentRepliesUseCase(
        ICommentRepository commentRepository,
        ILogger<GetCommentRepliesUseCase> logger
    )
    {
        _commentRepository = commentRepository;
        _logger = logger;
    }

    public async Task<GetCommentRepliesResponse> ExecuteAsync(GetCommentRepliesRequest request)
    {
        var result = await _commentRepository.GetRepliesAsync(request.ParentCommentId);

        if (!result.IsSuccess)
        {
            LogFetchFailure(result.Exception, request.ParentCommentId, result.ErrorMessage);
            return new GetCommentRepliesResponse(false,
                Message: "Failed to load replies.", ErrorType: result.ErrorType);
        }

        var dtos = CommentMapper.MapComments(result.Value!);

        return new GetCommentRepliesResponse(true, dtos);
    }
}