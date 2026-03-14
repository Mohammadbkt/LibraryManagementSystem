using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using library.Data;
using library.Dtos.Auth;
using library.Models.Configurations;
using library.Models.Entities;
using library.Services.Interface;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace library.Services.Implementation
{
    public class OtpService : IOtpService
    {

        private readonly AppDbContext _context;
        private readonly OtpConfig _otpConfig;


        public OtpService(AppDbContext context, IOptions<OtpConfig> otpConfig)
        {
            _context = context;
            _otpConfig = otpConfig.Value;
        }

        public async Task<OtpResultDto> GenerateOtpAsync(string userId, string? ipAddress = null)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var activeOtps = await _context.Otps
                    .Where(o => o.UserId == userId && !o.IsUsed && o.ExpiresAt > DateTime.UtcNow)
                    .ToListAsync();

                foreach (var otp in activeOtps)
                {
                    otp.IsUsed = true;
                }

                var otpCode = GenerateSecureOtp(_otpConfig.OtpLength);

                var otpHash = HashOtp(otpCode);

                var otpToAdd = new Otp
                {
                    UserId = userId,
                    OtpHash = otpHash,
                    CreatedAt = DateTime.UtcNow,
                    ExpiresAt = DateTime.UtcNow.AddMinutes(_otpConfig.ExpiryMinutes),
                    IsUsed = false,
                    FailedAttempts = 0,
                    IpAddress = ipAddress,
                    RequestId = Guid.NewGuid().ToString()
                };

                await _context.Otps.AddAsync(otpToAdd);
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();

                return new OtpResultDto
                {
                    OtpCode = otpCode,
                    ExpiresAt = otpToAdd.ExpiresAt,
                    ExpiresInMinutes = _otpConfig.ExpiryMinutes
                };

            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                System.Console.WriteLine($"{ex.Message}");
                throw;
            }

        }

        private static string HashOtp(string otp)
        {
            var hash = SHA256.HashData(Encoding.UTF8.GetBytes(otp));

            var hashByte = Encoding.UTF8.GetBytes(Convert.ToBase64String(hash));
            var finalHash = SHA256.HashData(hashByte);

            return Convert.ToBase64String(finalHash);
        }

        private static string GenerateSecureOtp(int otpLength)
        {
            using var rng = RandomNumberGenerator.Create();
            var bytes = new byte[4];
            rng.GetBytes(bytes);

            var value = BitConverter.ToUInt32(bytes, 0);
            var min = (int)Math.Pow(10, otpLength - 1);
            var max = (int)Math.Pow(10, otpLength) - 1;

            var otp = (value % (max - min + 1) + min).ToString();
            return otp.PadLeft(otpLength, '0');
        }

        public async Task<bool> VerifyOtpAsync(string userId, string otpCode, string? ipAddress = null)
        {
            // Basic validation
            if (string.IsNullOrWhiteSpace(otpCode) || otpCode.Length != _otpConfig.OtpLength)
            {
                return false;
            }

            var otp = await _context.Otps
                .Where(o => o.UserId.Equals(userId) && !o.IsUsed)
                .OrderByDescending(o => o.CreatedAt)
                .FirstOrDefaultAsync();

            if (otp == null)
            {
                return false;
            }

            // Check if expired
            if (otp.ExpiresAt <= DateTime.UtcNow)
            {
                otp.IsUsed = true; // Mark as used to prevent further attempts
                await _context.SaveChangesAsync();
                return false;
            }

            // Check IP if required
            if (_otpConfig.RequireIpValidation && !string.IsNullOrEmpty(otp.IpAddress) && otp.IpAddress != ipAddress)
            {
                otp.FailedAttempts++;
                await _context.SaveChangesAsync();
                return false;
            }

            if (otp.FailedAttempts >= _otpConfig.MaxFailedAttempts)
            {
                otp.IsUsed = true;
                await _context.SaveChangesAsync();
                return false;
            }

            var isValid = VerifyOtpHash(otpCode, otp.OtpHash);

            if (isValid)
            {
                otp.IsUsed = true;
                otp.VerifiedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();

            }

            otp.FailedAttempts++;
            await _context.SaveChangesAsync();


            return false;
        }

        public async Task<int> CleanupExpiredOtpsAsync()
        {
            var expiredOtps = await _context.Otps
                .Where(o => o.ExpiresAt < DateTime.UtcNow.AddDays(-7))
                .ToListAsync();

            _context.Otps.RemoveRange(expiredOtps);
            var deleted = await _context.SaveChangesAsync();
            
            return deleted;
        }

        private static bool VerifyOtpHash(string otp, string storedHash)
        {
            try
            {
                var computedHash = HashOtp(otp);

                return CryptographicOperations.FixedTimeEquals(
                    Encoding.UTF8.GetBytes(computedHash),
                    Encoding.UTF8.GetBytes(storedHash));
            }
            catch
            {
                return false;
            }
        }
    }
}