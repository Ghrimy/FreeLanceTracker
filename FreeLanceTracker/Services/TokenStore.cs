using System.Security.Claims;
using FreeLanceTracker.Data;
using FreeLanceTracker.Services.JwtTokenService;
using Microsoft.AspNetCore.Identity;

namespace FreeLanceTracker.Services;


public class TokenStore(
    UserManager<ApplicationUser> userManager,
    IJwtTokenService jwtTokenService)
{
    private string? _token;

    public async Task InitializeAsync(ClaimsPrincipal user)
    {
        Console.WriteLine("=== TokenStore.InitializeAsync ===");

        Console.WriteLine(
            $"Authenticated: {user.Identity?.IsAuthenticated}");

        Console.WriteLine(
            $"AuthenticationType: {user.Identity?.AuthenticationType}");

        Console.WriteLine(
            $"UserName: {user.Identity?.Name}");

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

        Console.WriteLine(
            $"ApplicationUser found: {applicationUser.UserName}");

        var (token, expiresAt) =
            jwtTokenService.GenerateToken(applicationUser);

        _token = token;

        Console.WriteLine(
            $"JWT generated. Expires: {expiresAt}");
    }

    public Task<string?> GetTokenAsync()
    {
        Console.WriteLine(
            $"GetTokenAsync: token exists = {_token is not null}");

        return Task.FromResult(_token);
    }
}