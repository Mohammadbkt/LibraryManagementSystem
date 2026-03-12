using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using library.Data;
using library.Dtos.Token;
using library.Models.Configurations;
using library.Models.Entities;
using library.Services.Interface;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace library.Services.Implementation
{
    public class JwtService : IJwtService
    {
        private readonly UserManager<User> _userManager;
        private readonly JwtConfig _jwtConfig;
        private readonly AppDbContext _context;

        public JwtService(UserManager<User> userManager, IOptions<JwtConfig> jwtOptions, AppDbContext context)
        {
            _userManager = userManager;
            _jwtConfig = jwtOptions.Value;
            _context = context;
        }

        public async Task<TokenResponseDto> GenerateTokenAsync(User user, string deviceName, string ipAddress)
        {
            var accessToken = await GenerateAccessTokenAsync(user);
            var refreshToken = await GenerateAndStoreRefreshTokenAsync(user, deviceName, ipAddress);
            var userRoles = await _userManager.GetRolesAsync(user);

            return new TokenResponseDto
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken.Token,
                ExpiresAt = DateTime.UtcNow.AddMinutes(_jwtConfig.TokenValidityInMinutes),
                UserId = user.Id,
                Email = user.Email!,
                Roles = userRoles.ToList()
            };
        }

        private async Task<string> GenerateAccessTokenAsync(User user)
        {
            var userRoles = await _userManager.GetRolesAsync(user);

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id),
                new Claim(JwtRegisteredClaimNames.Email, user.Email ?? ""),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.NameIdentifier, user.Id)
            };

            if (!string.IsNullOrEmpty(user.FirstName))
                claims.Add(new Claim("firstName", user.FirstName));

            if (!string.IsNullOrEmpty(user.LastName))
                claims.Add(new Claim("lastName", user.LastName));

            foreach (var userRole in userRoles)
                claims.Add(new Claim(ClaimTypes.Role, userRole));

            var expiresAt = DateTime.UtcNow.AddMinutes(_jwtConfig.TokenValidityInMinutes);

            var token = new JwtSecurityToken(
                issuer: _jwtConfig.ValidIssuer,
                audience: _jwtConfig.ValidAudience,
                claims: claims,
                expires: expiresAt,
                signingCredentials: new SigningCredentials(
                    new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtConfig.Secret)),
                    SecurityAlgorithms.HmacSha256)
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private async Task<RefreshToken> GenerateAndStoreRefreshTokenAsync(User user, string deviceName, string ipAddress)
        {
            var refreshToken = new RefreshToken
            {
                UserId = user.Id,
                Token = GenerateRefreshToken(),
                DeviceName = deviceName,
                IpAddress = ipAddress,
                ExpiresAt = DateTime.UtcNow.AddDays(_jwtConfig.RefreshTokenValidityInDays),
                CreatedAt = DateTime.UtcNow
            };

            await _context.RefreshTokens.AddAsync(refreshToken);
            await _context.SaveChangesAsync();

            return refreshToken;
        }

        public string GenerateRefreshToken()
        {
            var randomNumber = new byte[64];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }

        public async Task<TokenResponseDto?> RefreshTokenAsync(string accessToken, string refreshToken, string deviceName, string ipAddress)
        {
            var principal = ValidateToken(accessToken);
            if (principal == null)
                return null;

            var userId = principal.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
                return null;

            var storedToken = await _context.RefreshTokens
                .Include(rt => rt.User)
                .FirstOrDefaultAsync(rt => rt.Token == refreshToken && rt.UserId == userId);

            if (storedToken == null)
                return null;

            // Reuse detection — token already revoked means possible theft
            if (storedToken.RevokedAt != null)
            {
                await RevokeAllUserTokensAsync(userId);
                return null;
            }

            if (!storedToken.IsActive)
                return null;

            // Rotate the refresh token
            var newRefreshToken = await GenerateAndStoreRefreshTokenAsync(storedToken.User, deviceName, ipAddress);
            storedToken.RevokedAt = DateTime.UtcNow;
            storedToken.ReplacedByToken = newRefreshToken.Token;
            await _context.SaveChangesAsync();

            // Generate new access token only
            var newAccessToken = await GenerateAccessTokenAsync(storedToken.User);
            var userRoles = await _userManager.GetRolesAsync(storedToken.User);

            return new TokenResponseDto
            {
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken.Token,
                ExpiresAt = DateTime.UtcNow.AddMinutes(_jwtConfig.TokenValidityInMinutes),
                UserId = storedToken.UserId,
                Email = storedToken.User.Email!,
                Roles = userRoles.ToList()
            };
        }

        public ClaimsPrincipal? ValidateToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtConfig.Secret));

            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = key,
                ValidateIssuer = true,
                ValidIssuer = _jwtConfig.ValidIssuer,
                ValidateAudience = true,
                ValidAudience = _jwtConfig.ValidAudience,
                ValidateLifetime = false, // intentional — handles expired tokens during refresh
                ClockSkew = TimeSpan.Zero
            };

            try
            {
                var principal = tokenHandler.ValidateToken(token, validationParameters, out SecurityToken validatedToken);

                if (validatedToken is not JwtSecurityToken jwtToken ||
                    !jwtToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                    return null;

                return principal;
            }
            catch
            {
                return null;
            }
        }

        private async Task RevokeAllUserTokensAsync(string userId)
        {
            var activeTokens = await _context.RefreshTokens
                .Where(rt => rt.UserId == userId && rt.RevokedAt == null)
                .ToListAsync();

            foreach (var token in activeTokens)
                token.RevokedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
        }
    }
}