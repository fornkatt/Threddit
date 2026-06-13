using System.Collections.Immutable;
using Microsoft.Extensions.Logging;
using Threddit.Application.DTOs.Common;
using Threddit.Application.DTOs.Requests.SubThreads;
using Threddit.Application.DTOs.Responses.SubThreads;
using Threddit.Application.Interfaces.Driven;
using Threddit.Application.Interfaces.Driving;
using Threddit.Domain.Common;

namespace Threddit.Application.UseCases.SubThreads;

public sealed partial class GetSubThreadByNameUseCase : IGetSubThreadByNameUseCase
{
    private readonly ISubThreadRepository _subThreadRepository;
    private readonly ILogger<GetSubThreadByNameUseCase> _logger;

    public GetSubThreadByNameUseCase(
        ISubThreadRepository subThreadRepository,
        ILogger<GetSubThreadByNameUseCase> logger
    )
    {
        _subThreadRepository = subThreadRepository;
        _logger = logger;
    }

    public async Task<GetSubThreadByNameResponse> ExecuteAsync(GetSubThreadByNameRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
            return new GetSubThreadByNameResponse(false, Message: "SubThread name cannot be empty.",
                ErrorType: ErrorType.NameEmpty);

        var result = await _subThreadRepository.GetByNameAsync(request.Name);
        if (!result.IsSuccess)
        {
            LogNotFound(result.Exception, request.Name, result.ErrorMessage);
            return new GetSubThreadByNameResponse(false, Message: $"No SubThread found with name '{request.Name}'.",
                ErrorType: result.ErrorType);
        }

        var subThread = result.Value!;

        var dto = new SubThreadDto(
            subThread.Id,
            subThread.CreatedBy?.UserName,
            subThread.Name,
            subThread.Description,
            subThread.BannerImageUrl,
            subThread.SubscriberCount,
            subThread.CreatedAt,
            subThread.UpdatedAt,
            subThread.SubThreadRules
                .Select(r => new SubThreadRuleDto(r.Id, r.RuleTitle, r.RuleContent))
                .ToImmutableList()
        );

        return new GetSubThreadByNameResponse(true, dto);
    }
}