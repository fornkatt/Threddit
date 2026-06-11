using Microsoft.Extensions.Logging;
using Threddit.Application.DTOs.Common;
using Threddit.Application.DTOs.Requests.SubThreads;
using Threddit.Application.DTOs.Responses.SubThreads;
using Threddit.Application.Interfaces.Driven;
using Threddit.Application.Interfaces.Driving;
using Threddit.Domain.Common;

namespace Threddit.Application.UseCases.SubThreads;

public sealed partial class GetSubThreadPostsUseCase : IGetSubThreadPostsUseCase
{
    private readonly ISubThreadRepository _subThreadRepository;
    private readonly ILogger<GetSubThreadPostsUseCase> _logger;

    public GetSubThreadPostsUseCase(
        ISubThreadRepository subThreadRepository,
        ILogger<GetSubThreadPostsUseCase> logger
    )
    {
        _subThreadRepository = subThreadRepository;
        _logger = logger;
    }

    public async Task<GetSubThreadPostsResponse> ExecuteAsync(GetSubThreadPostsRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.SubThreadName))
            return new GetSubThreadPostsResponse(
                false,
                Message: "SubThread name cannot be empty.",
                ErrorType: ErrorType.NameEmpty);

        var page = Math.Max(1, request.Page);
        var pageSize = Math.Clamp(request.PageSize, 1, 50);

        var subThreadResult = await _subThreadRepository.GetByNameAsync(request.SubThreadName);
        if (!subThreadResult.IsSuccess)
        {
            LogNotFound(subThreadResult.Exception ,request.SubThreadName, subThreadResult.ErrorMessage);
            return new GetSubThreadPostsResponse(
                false,
                Message: "SubThread not found.",
                ErrorType: subThreadResult.ErrorType);
        }

        var postsResult = await _subThreadRepository.GetPostsAsync(subThreadResult.Value!.Id,
            page, pageSize, request.SortOrder);
        if (!postsResult.IsSuccess)
        {
            LogFailedToLoadPosts(subThreadResult.Exception, request.SubThreadName, postsResult.ErrorMessage);
            return new GetSubThreadPostsResponse(
                false,
                Message: "Failed to load posts. Please try again later.",
                ErrorType: postsResult.ErrorType);
        }

        var paged = postsResult.Value!;

        var dto = new PagedResult<PostDto>(
            paged.Items.Select(p => new PostDto(
                p.Id,
                p.SubThreadId,
                p.PostedById,
                p.PostedBy?.UserName,
                p.PostedBy?.ProfilePicture,
                p.Title ?? string.Empty,
                p.Content,
                p.ImageUrl,
                p.Slug,
                p.Score,
                p.CommentCount,
                p.PostedAt,
                p.UpdatedAt
                )).ToList().AsReadOnly(),
            paged.Page,
            paged.PageSize,
            paged.TotalCount
            );
        
        return new GetSubThreadPostsResponse(true, dto);
    }
}