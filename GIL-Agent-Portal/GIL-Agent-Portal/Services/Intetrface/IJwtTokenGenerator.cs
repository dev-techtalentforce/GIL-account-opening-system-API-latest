using GIL_Agent_Portal.DTOs;
using GIL_Agent_Portal.Models;
using System.Security.Claims;

namespace GIL_Agent_Portal.Services.Intetrface
{
    public interface IJwtTokenGenerator
    {
        string GenerateToken(UserLoginResponseDto user);
        string GenerateRefreshToken();
        ClaimsPrincipal? GetPrincipalFromExpiredToken(string token);
    }
}
