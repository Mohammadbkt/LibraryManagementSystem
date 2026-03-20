using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using library.Dtos.Auth;
using library.Dtos.Token;
using library.Models.Entities;

namespace library.Services.Interface
{
    public interface IJwtService
    {
        Task<TokenResponseDto> GenerateTokenAsync(User user, string? deviceName, string? ipAddress);
        string GenerateRefreshToken();
        Task<TokenResponseDto?> RefreshTokenAsync(string accessToken, string refreshToken, string? deviceName, string? ipAddress);
        ClaimsPrincipal? ValidateToken(string token);

        Task RevokeTokenAsync(string userId, string refreshToken);
        Task RevokeAllTokensAsync(string userId);
    }
}