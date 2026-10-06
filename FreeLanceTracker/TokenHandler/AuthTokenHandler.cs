using System.Net.Http.Headers;
using FreeLanceTracker.Services;

namespace FreeLanceTracker.TokenHandler;

/// <summary>
/// Adds the JWT token to the request headers.
/// </summary>
/// <param name="tokenStore"></param>
public class AuthTokenHandler(TokenStore tokenStore) : DelegatingHandler
{
    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        var token = await tokenStore.GetTokenAsync();

        if (!string.IsNullOrEmpty(token))
        {
            request.Headers.Authorization =
                new AuthenticationHeaderValue("Bearer", token);
        }

        return await base.SendAsync(request, cancellationToken);
    }
}