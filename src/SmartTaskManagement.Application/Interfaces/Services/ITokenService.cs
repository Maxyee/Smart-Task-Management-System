using System.Security.Claims;

namespace SmartTaskManagement.Application.Interfaces.Services
{
    public interface ITokenService
    {
        string GenerateAccessToken(IEnumerable<Claim> claims);
        string GenerateRefreshToken();
        ClaimsPrincipal GetPrincipalFromExpiredToken(string token);
        (string AccessToken, string RefreshToken) GenerateTokens(IEnumerable<Claim> claims);
        int GetRefreshTokenExpiryDays();
    }

}