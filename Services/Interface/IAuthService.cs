using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using library.Dtos.Auth;

namespace library.Services.Interface
{
    public interface IAuthService
    {
        Task<AuthResponseDto?> RegisterAsync(RegisterDto registerDto, string deviceName, string ipAddress);
        Task<AuthResponseDto?> LoginAsync(LoginDto loginDto, string deviceName, string ipAddress);
        Task<AuthResponseDto?> RefreshTokenAsync(string accessToken, string refreshToken, string deviceName, string ipAddress);
        Task<bool> LogoutAsync(string userId, string refreshToken);
        Task<bool> LogoutAllDevicesAsync(string userId);
        Task<bool> ChangePasswordAsync(string userId, string currentPassword, string newPassword);

    }
}