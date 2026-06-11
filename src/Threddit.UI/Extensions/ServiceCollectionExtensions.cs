using Microsoft.AspNetCore.Components.Authorization;
using Threddit.UI.ApiClient;
using Threddit.UI.Auth;
using Threddit.UI.Interfaces;
using Threddit.UI.Stores;

namespace Threddit.UI.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddUIServices(this IServiceCollection services,
        IConfiguration configuration)
    {
        services
            .AddScoped<ICurrentUserStore, CurrentUserStore>()
            .AddScoped<ITokenStore, LocalStorageTokenStore>()
            .AddScoped<AuthTokenHandler>()
            .AddAuthorizationCore()
            .AddScoped<AuthenticationStateProvider, JwtAuthenticationStateProvider>()
            .AddScoped(sp => (JwtAuthenticationStateProvider)sp.GetRequiredService<AuthenticationStateProvider>())
            
            .AddHttpClient<ThredditApiClient>(client =>
            {
                client.BaseAddress = new Uri(configuration["ApiBaseUrl"] ?? "https://localhost:7227");
            })
            .AddHttpMessageHandler<AuthTokenHandler>();

        return services;
    }
}