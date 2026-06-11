using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Threddit.Application.Interfaces.Driven;
using Threddit.Infrastructure.Persistence;
using Threddit.Infrastructure.Persistence.Repositories;
using Threddit.Infrastructure.Services;

namespace Threddit.Infrastructure.Extensions;

public static class ServiceCollectionExtension
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContextFactory<ThredditDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

        services.AddScoped<ThredditDbContext>(sp =>
            sp.GetRequiredService<IDbContextFactory<ThredditDbContext>>().CreateDbContext());

        services
            .AddScoped<IDbSeeder, DbSeeder>()
            .AddScoped<ISubThreadRepository, SubThreadRepository>()
            .AddScoped<IPostRepository, PostRepository>()
            .AddScoped<ICommentRepository, CommentRepository>()
            .AddScoped<IUserRepository, UserRepository>()
            .AddScoped<IConversationRepository, ConversationRepository>()
            .AddScoped<IReportRepository, ReportRepository>()
            .AddScoped<IJwtService, JwtService>();

        var useAzureBlob = configuration["ImageStorage:UseAzureBlob"];
        if (useAzureBlob == "true")
        {
            var connectionsString = configuration["ImageStorage:AzureBlobConnectionString"]!;
            var containerName = configuration["ImageStorage:ContainerName"] ?? "images";
            services.AddSingleton<IImageStorageService>(
                new AzureBlobImageStorageService(connectionsString, containerName));
        }
        else
        {
            var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
            var baseUrl = configuration["ImageStorage:LocalBaseUrl"] ?? "https://localhost:7001";
            services.AddSingleton<IImageStorageService>(new LocalImageStorageService(uploadPath, baseUrl));
        }

        return services;
    }
}