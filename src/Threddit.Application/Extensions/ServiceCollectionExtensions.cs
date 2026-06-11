using Microsoft.Extensions.DependencyInjection;
using Threddit.Application.Interfaces.Driving;
using Threddit.Application.UseCases.Auth;
using Threddit.Application.UseCases.Comments;
using Threddit.Application.UseCases.Posts;
using Threddit.Application.UseCases.Reports;
using Threddit.Application.UseCases.SubThreads;
using Threddit.Application.UseCases.Users;
using Threddit.Application.UseCases.Validation;

namespace Threddit.Application.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services
            .AddScoped<ICreatePostUseCase, CreatePostUseCase>()
            .AddScoped<IDeletePostUseCase, DeletePostUseCase>()
            .AddScoped<IGetPostByIdWithCommentsUseCase, GetPostByIdWithCommentsUseCase>()
            .AddScoped<IVotePostUseCase, VotePostUseCase>()
            .AddScoped<IEditPostUseCase, EditPostUseCase>()
            
            .AddScoped<IGetCommentRepliesUseCase, GetCommentRepliesUseCase>()
            .AddScoped<ICreateCommentUseCase, CreateCommentUseCase>()
            .AddScoped<IVoteCommentUseCase, VoteCommentUseCase>()
            .AddScoped<IDeleteCommentUseCase, DeleteCommentUseCase>()
            .AddScoped<IEditCommentUseCase, EditCommentUseCase>()
            
            .AddScoped<IGetSubThreadsBySearchUseCase, GetSubThreadsBySearchUseCase>()
            .AddScoped<IGetSubThreadByNameUseCase, GetSubThreadByNameUseCase>()
            .AddScoped<IGetSubThreadPostsUseCase, GetSubThreadPostsUseCase>()
            .AddScoped<ICreateSubThreadUseCase, CreateSubThreadUseCase>()
            .AddScoped<ISubscribeSubThreadUseCase, SubscribeSubThreadUseCase>()
            .AddScoped<IUnsubscribeSubThreadUseCase, UnsubscribeSubThreadUseCase>()
            .AddScoped<IDeleteSubThreadUseCase, DeleteSubThreadUseCase>()
            .AddScoped<IEditSubThreadUseCase, EditSubThreadUseCase>()
            
            .AddScoped<ICreateSubThreadRuleUseCase, CreateSubThreadRuleUseCase>()
            .AddScoped<IDeleteSubThreadRuleUseCase, DeleteSubThreadRuleUseCase>()
            .AddScoped<IEditSubThreadRuleUseCase, EditSubThreadRuleUseCase>()
            
            .AddScoped<IBanSiteUserUseCase, BanSiteUserUseCase>()
            .AddScoped<IUnbanSiteUserUseCase, UnbanSiteUserUseCase>()
            .AddScoped<IBanSubThreadUserUseCase, BanSubThreadUserUseCase>()
            .AddScoped<IUnbanSubThreadUserUseCase, UnbanSubThreadUserUseCase>()
            .AddScoped<IGetSubThreadModeratorsUseCase, GetSubThreadModeratorsUseCase>()
            .AddScoped<IAssignModeratorUseCase, AssignModeratorUseCase>()
            .AddScoped<IRemoveModeratorUseCase, RemoveModeratorUseCase>()
            
            .AddScoped<IAssignSiteAdminUseCase, AssignSiteAdminUseCase>()
            .AddScoped<IRemoveSiteAdminUseCase, RemoveSiteAdminUseCase>()
            .AddScoped<IGetAllSiteAdminsUseCase, GetAllSiteAdminsUseCase>()
            
            .AddScoped<ICreateSiteReportUseCase, CreateSiteReportUseCase>()
            .AddScoped<ICreateSubThreadReportUseCase, CreateSubThreadReportUseCase>()
            .AddScoped<IGetSiteReportsUseCase, GetSiteReportsUseCase>()
            .AddScoped<IGetSubThreadReportsUseCase, GetSubThreadReportsUseCase>()
            .AddScoped<ISetReportStatusUseCase, SetReportStatusUseCase>()
            
            .AddScoped<IGetUserProfileUseCase, GetUserProfileUseCase>()
            .AddScoped<IGetCurrentUserSummaryUseCase, GetCurrentUserSummaryUseCase>()
            .AddScoped<IEditUserProfileUseCase, EditUserProfileUseCase>()
            
            .AddScoped<IRegistrationUseCase, RegistrationUseCase>()
            .AddScoped<ILoginUseCase, LoginUseCase>()
            
            .AddScoped<IGetValidationLimitsUseCase, GetValidationLimitsUseCase>();

        return services;
    }
}