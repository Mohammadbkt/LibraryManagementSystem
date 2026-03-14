using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using library.Dtos.Auth;

namespace library.Services.Interface
{
    public interface IOtpService
    {
        Task<OtpResultDto> GenerateOtpAsync(string userId, string? ipAddress = null);
        Task<bool> VerifyOtpAsync(string userId, string otpCode, string? ipAddress = null);
        Task<int> CleanupExpiredOtpsAsync();
    }
}