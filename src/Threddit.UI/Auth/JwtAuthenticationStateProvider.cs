using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.IdentityModel.JsonWebTokens;

namespace Threddit.UI.Auth;

public sealed class JwtAuthenticationStateProvider : AuthenticationStateProvider
{
    private static readonly AuthenticationState Anonymous =
        new(new ClaimsPrincipal(new ClaimsIdentity()));

    private readonly ITokenStore _tokenStore;

    public JwtAuthenticationStateProvider(
        ITokenStore tokenStore
    )
    {
        _tokenStore = tokenStore;
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var token = await _tokenStore.GetTokenAsync();
        if (string.IsNullOrWhiteSpace(token))
            return Anonymous;

        try
        {
            var jwt = new JsonWebToken(token);

            if (jwt.ValidTo <= DateTime.UtcNow)
            {
                await _tokenStore.ClearAsync();
                return Anonymous;
            }

            var identity = new ClaimsIdentity(
                jwt.Claims,
                authenticationType: "jwt",
                nameType: "username",
                roleType: "role"
            );
            
            return new AuthenticationState(new ClaimsPrincipal(identity));
        }
        catch
        {
            await _tokenStore.ClearAsync();
            return Anonymous;
        }
    }

    public void NotifyUserAuthentication() =>
        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());

    public void NotifyUserLogout() =>
        NotifyAuthenticationStateChanged(Task.FromResult(Anonymous));
}