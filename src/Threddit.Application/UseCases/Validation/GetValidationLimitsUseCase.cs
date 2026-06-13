using Threddit.Application.DTOs.Responses.Validation;
using Threddit.Application.Interfaces.Driving;
using Threddit.Domain.Entities;

namespace Threddit.Application.UseCases.Validation;

public sealed class GetValidationLimitsUseCase : IGetValidationLimitsUseCase
{
    public GetValidationLimitsResponse Execute() => new(
        Post.Limits.MaxTitleLength,
        Post.Limits.MaxSlugLength,
        Post.Limits.MaxContentLength,
        Post.Limits.MaxImageUrlLength,
        Post.Limits.MaxDeleteReasonLength,
        Comment.Limits.MaxContentLength,
        Comment.Limits.MaxDeleteReasonLength,
        SubThread.Limits.MaxNameLength,
        SubThread.Limits.MaxDescriptionLength,
        SubThread.Limits.MaxBannerImageUrlLength,
        SubThreadRule.Limits.MaxTitleLength,
        SubThreadRule.Limits.MaxContentLength,
        User.Limits.MaxUsernameLength,
        User.Limits.MaxEmailLength,
        User.Limits.MaxProfilePictureUrlLength,
        User.Limits.MaxDescriptionLength,
        GroupConversation.Limits.MaxNameLength,
        DirectMessage.Limits.MaxContentLength,
        Report.Limits.MaxMessageLength
    );
}