using System.Security.Claims;
using System.Threading.Tasks;
using FreeLanceTracker.Data;
using FreeLanceTracker.Services.JwtTokenService;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace FreeLanceTracker.Services;

/// <summary>
///     Stores the JWT token for the current user.
/// </summary>
/// <param name="userManager"></param>
/// <param name="jwtTokenService"></param>
public class TokenStore(
    UserManager<ApplicationUser> userManager,
    IJwtTokenService jwtTokenService,
    ILogger<TokenStore> logger)
{
    private string? _token;

    public async Task InitializeAsync(ClaimsPrincipal user)
    {
        if (_token is not null)
        {
            logger.LogInformation("Token already exists.");
            return;
        }

        if (user.Identity?.IsAuthenticated != true)
        {
            logger.LogInformation("User is NOT authenticated.");
            return;
        }

        var applicationUser = await userManager.GetUserAsync(user);

        if (applicationUser is null)
        {
            logger.LogInformation("UserManager.GetUserAsync returned NULL.");
            return;
        }

        var (token, expiresAt) = jwtTokenService.GenerateToken(applicationUser);

        _token = token;
    }

    public Task<string?> GetTokenAsync()
    {
        return Task.FromResult(_token);
    }
}