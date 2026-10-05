using FreeLanceTracker.Data;

namespace FreeLanceTracker.Services.JwtTokenService;

public interface IJwtTokenService
{
    (string Token, DateTime ExpiresAt) GenerateToken(ApplicationUser user);
}