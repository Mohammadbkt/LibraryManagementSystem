using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using library.Dtos.Auth;

namespace library.Services.Interface
{
    public interface IAuthService
    {
        Task<RegisterResponseDto> RegisterAsync(RegisterDto registerDto, string deviceName, string ipAddress);
        Task<LoginResponseDto> LoginAsync(LoginDto loginDto, string? deviceName, string? ipAddress);
        Task<RefreshResponseDto> RefreshTokenAsync(string accessToken, string refreshToken, string? deviceName, string? ipAddress);
        Task<VerifyOtpResponseDto> VerifyOtpAsync(string userId, string otpCode, string? deviceName, string? ipAddress);
        Task<ResendOtpResponseDto> ResendOtpAsync(string userId);
        Task<bool> LogoutAsync(string userId, string refreshToken);
        Task<bool> LogoutAllDevicesAsync(string userId);
        Task<bool> ChangePasswordAsync(string userId, string currentPassword, string newPassword);

    }
}