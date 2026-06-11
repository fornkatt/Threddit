using System.Collections.Immutable;
using Microsoft.Extensions.Logging;
using Threddit.Application.DTOs.Common;
using Threddit.Application.DTOs.Responses.Users;
using Threddit.Application.Interfaces.Driven;
using Threddit.Application.Interfaces.Driving;
using Threddit.Domain.Common;

namespace Threddit.Application.UseCases.Users;

public sealed partial class GetAllSiteAdminsUseCase : IGetAllSiteAdminsUseCase
{
    private readonly IUserRepository _userRepository;
    private readonly ILogger<GetAllSiteAdminsUseCase> _logger;

    public GetAllSiteAdminsUseCase(
        IUserRepository userRepository,
        ILogger<GetAllSiteAdminsUseCase> logger
    )
    {
        _userRepository = userRepository;
        _logger = logger;
    }

    public async Task<GetAllSiteAdminsResponse> ExecuteAsync()
    {
        var result = await _userRepository.GetSiteAdminsAsync();
        if (!result.IsSuccess)
        {
            LogFetchFailure(result.Exception, result.ErrorMessage);
            var message = ResolveErrorMessage(result.ErrorType);
            return new GetAllSiteAdminsResponse(false, [], message, result.ErrorType);
        }

        var dtos = result.Value!
            .Select(sa => new SiteAdminDto(
                sa.Id,
                sa.User.UserName!,
                sa.AssignedAt,
                sa.User.IssuedSiteBans
                    .Select(ban => new IssuedSiteBanDto(
                        ban.Id,
                        ban.UserId,
                        ban.User.UserName!,
                        ban.Reason,
                        ban.BannedAt,
                        ban.ExpiresAt,
                        ban.IsExpired
                    )).ToImmutableList()
            )).ToImmutableList();
        
        return new GetAllSiteAdminsResponse(true, dtos);
    }

    private static string ResolveErrorMessage(ErrorType errorType) => errorType switch
    {
        ErrorType.DatabaseTimeout => "The request timed out. Please try again later.",
        _ => "An unexpected error occurred. Please try again later."
    };
}