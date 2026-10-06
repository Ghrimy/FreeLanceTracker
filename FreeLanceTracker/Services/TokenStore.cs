using System.Security.Claims;
using FreeLanceTracker.Data;
using FreeLanceTracker.Services.JwtTokenService;
using Microsoft.AspNetCore.Identity;

namespace FreeLanceTracker.Services;

/// <summary>
/// Stores the JWT token for the current user.
/// </summary>
/// <param name="userManager"></param>
/// <param name="jwtTokenService"></param>
public class TokenStore(
    UserManager<ApplicationUser> userManager,
    IJwtTokenService jwtTokenService)
{
    private string? _token;

    public async Task InitializeAsync(ClaimsPrincipal user)
    {
        if (_token is not null)
        {
            Console.WriteLine("Token already exists.");
            return;
        }

        if (user.Identity?.IsAuthenticated != true)
        {
            Console.WriteLine("User is NOT authenticated.");
            return;
        }

        var applicationUser = await userManager.GetUserAsync(user);

        if (applicationUser is null)
        {
            Console.WriteLine("UserManager.GetUserAsync returned NULL.");
            return;
        }

        var (token, expiresAt) =
            jwtTokenService.GenerateToken(applicationUser);

        _token = token;
    }

    public Task<string?> GetTokenAsync()
    {
        Console.WriteLine(
            $"GetTokenAsync: token exists = {_token is not null}");

        return Task.FromResult(_token);
    }
}