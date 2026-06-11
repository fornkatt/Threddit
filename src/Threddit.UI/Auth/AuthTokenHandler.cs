using System.Net.Http.Headers;

namespace Threddit.UI.Auth;

public sealed class AuthTokenHandler : DelegatingHandler
{
    private readonly ITokenStore _tokenStore;

    public AuthTokenHandler(
        ITokenStore tokenStore
    )
    {
        _tokenStore = tokenStore;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken ct)
    {
        var token = await _tokenStore.GetTokenAsync();
        if (!string.IsNullOrWhiteSpace(token))
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        return await base.SendAsync(request, ct);
    }
}