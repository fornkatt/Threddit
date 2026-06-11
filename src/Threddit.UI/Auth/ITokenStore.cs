using Threddit.Contracts.Responses.Auth;

namespace Threddit.UI.Auth;

public interface ITokenStore
{
    ValueTask<string?> GetTokenAsync();
    ValueTask<string?> GetUsernameAsync();
    ValueTask SetAsync(LoginApiResponse login);
    ValueTask ClearAsync();
}