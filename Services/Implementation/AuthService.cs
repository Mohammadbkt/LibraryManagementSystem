using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using library.Data;
using library.Dtos.Auth;
using library.Models.Entities;
using library.Services.Interface;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace library.Services.Implementation
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<User> _userManager;
        private readonly IOtpService _otpService;
        private readonly IEmailService _emailService;
        private readonly SignInManager<User> _signInManager;
        private readonly IJwtService _jwtService;

        public AuthService(UserManager<User> userManager, IOtpService otpService, IEmailService emailService, SignInManager<User> signInManager, IJwtService jwtService)
        {
            _userManager = userManager;
            _otpService = otpService;
            _emailService = emailService;
            _signInManager = signInManager;
            _jwtService = jwtService;
        }

        public async Task<LoginResponseDto> LoginAsync(LoginDto loginDto, string? deviceName, string? ipAddress)
        {
            var user = await _userManager.FindByEmailAsync(loginDto.Email);
            if (user == null)
            {
                return new LoginResponseDto
                {
                    IsSuccess = false,
                    Message = "There is no account with these credentials."
                };
            }

            if (!user.EmailConfirmed)
            {
                return new LoginResponseDto
                {
                    IsSuccess = false,
                    Message = "Confirm your account, an otp sent to your email."
                };
            }

            if (!user.IsActive)
            {
                return new LoginResponseDto
                {
                    IsSuccess = false,
                    Message = "account is deactivated"
                };
            }

            var result = await _signInManager.CheckPasswordSignInAsync(user, loginDto.Password, false);
            if (!result.Succeeded)
            {
                return new LoginResponseDto
                {
                    IsSuccess = false,
                    Message = "credentials are not correct"
                };
            }

            var tokenResponseDto = await _jwtService.GenerateTokenAsync(user, deviceName, ipAddress);

            return new LoginResponseDto
            {
                IsSuccess = true,
                AccessToken = tokenResponseDto.AccessToken,
                RefreshToken = tokenResponseDto.RefreshToken,
                ExpiresAt = tokenResponseDto.ExpiresAt,
                UserId = tokenResponseDto.UserId,
                Email = tokenResponseDto.Email,
                Roles = tokenResponseDto.Roles,
            };
        }

        public async Task<bool> LogoutAllDevicesAsync(string userId)
        {
            await _jwtService.RevokeAllTokensAsync(userId);
            return true;
        }

        public async Task<bool> LogoutAsync(string userId, string refreshToken)
        {
            await _jwtService.RevokeTokenAsync(userId, refreshToken);
            return true;
        }

        public async Task<RefreshResponseDto> RefreshTokenAsync(string accessToken, string refreshToken, string? deviceName, string? ipAddress)
        {

            var tokenResponse = await _jwtService.RefreshTokenAsync(accessToken, refreshToken, deviceName, ipAddress);

            if (tokenResponse == null)
                return new RefreshResponseDto { IsSuccess = false, Message = "Invalid or expired refresh token." };

            return new RefreshResponseDto
            {
                IsSuccess = true,
                AccessToken = tokenResponse.AccessToken,
                RefreshToken = tokenResponse.RefreshToken,
                ExpiresAt = tokenResponse.ExpiresAt,
            };

        }

        public async Task<RegisterResponseDto> RegisterAsync(RegisterDto registerDto, string deviceName, string ipAddress)
        {
            var existingUser = await _userManager.FindByEmailAsync(registerDto.Email);
            if (existingUser != null)
            {
                if (!existingUser.EmailConfirmed)
                {
                    try
                    {
                        var otp = await _otpService.GenerateOtpAsync(existingUser.Id);

                        await _emailService.SendEmailAsync(
                            existingUser.Email!,
                            "OTP Verification",
                            $"Hello {existingUser.FullName}, your verification code is: {otp.OtpCode}. This code will expire in {otp.ExpiresInMinutes} minutes."

                        );

                        return new RegisterResponseDto
                        {
                            IsSuccess = false,
                            Message = "Confirm you account, an otp sent to you email"
                        };
                    }
                    catch (Exception)
                    {
                        return new RegisterResponseDto
                        {
                            IsSuccess = false,
                            Message = "Failed to send verification email. Please try again."
                        };
                    }
                }

                return new RegisterResponseDto
                {
                    IsSuccess = false,
                    Message = "Account already exist"

                };
            }

            var existingUserName = await _userManager.FindByNameAsync(registerDto.UserName);
            if (existingUserName != null)
            {
                return new RegisterResponseDto
                {
                    IsSuccess = false,
                    Message = "Username already taken"
                };
            }

            var user = new User
            {
                UserName = registerDto.UserName,
                Email = registerDto.Email,
                PhoneNumber = registerDto.PhoneNumber,
                IsActive = false,
                EmailConfirmed = false
            };

            var result = await _userManager.CreateAsync(user, registerDto.Password);
            if (!result.Succeeded)
            {
                return new RegisterResponseDto
                {
                    IsSuccess = false,
                    Message = string.Join(", ", result.Errors.Select(e => e.Description))
                };
            }

            // Assign default role
            await _userManager.AddToRoleAsync(user, "USER");

            try
            {
                var otp = await _otpService.GenerateOtpAsync(user.Id);
                await _emailService.SendEmailAsync(
                    user.Email!,
                            "OTP Verification",
                            $"Hello {user.FullName}, your verification code is: {otp.OtpCode}. This code will expire in {otp.ExpiresInMinutes} minutes."

                );
            }
            catch (Exception)
            {
                await _userManager.DeleteAsync(user);

                return new RegisterResponseDto
                {
                    IsSuccess = false,
                    Message = "Failed to send verification email. Please try again."
                };
            }

            return new RegisterResponseDto
            {
                IsSuccess = true,
                UserId = user.Id,
                Email = user.Email,
                Message = "registeration successful"
            };
        }

        public async Task<bool> ChangePasswordAsync(string userId, string currentPassword, string newPassword)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
                return false;

            var result = await _userManager.ChangePasswordAsync(user, currentPassword, newPassword);
            return result.Succeeded;
        }

        public async Task<VerifyOtpResponseDto> VerifyOtpAsync(string userId, string otpCode, string? deviceName, string? ipAddress)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return new VerifyOtpResponseDto { IsSuccess = false, Message = "User not found" };

            if (user.EmailConfirmed)
                return new VerifyOtpResponseDto { IsSuccess = false, Message = "Account already verified" };

            var isValid = await _otpService.VerifyOtpAsync(userId, otpCode);
            if (!isValid)
                return new VerifyOtpResponseDto { IsSuccess = false, Message = "Invalid or expired OTP" };

            user.IsActive = true;
            user.EmailConfirmed = true;
            await _userManager.UpdateAsync(user);

            var tokenResponse = await _jwtService.GenerateTokenAsync(user, deviceName, ipAddress);

            return new VerifyOtpResponseDto
            {
                IsSuccess = true,
                Message = "Account verified successfully",
                AccessToken = tokenResponse.AccessToken,
                RefreshToken = tokenResponse.RefreshToken,
                ExpiresAt = tokenResponse.ExpiresAt
            };
        }

        public async Task<ResendOtpResponseDto> ResendOtpAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return new ResendOtpResponseDto { IsSuccess = false, Message = "User not found" };

            if (user.EmailConfirmed)
                return new ResendOtpResponseDto { IsSuccess = false, Message = "Account already verified" };

            try
            {
                var otp = await _otpService.GenerateOtpAsync(userId);
                await _emailService.SendEmailAsync(
                    user.Email!,
                    "OTP Verification",
                    $"Hello {user.FullName}, your new verification code is: {otp.OtpCode}. This code will expire in {otp.ExpiresInMinutes} minutes."
                );

                return new ResendOtpResponseDto { IsSuccess = true, Message = "OTP sent successfully" };
            }
            catch (Exception)
            {
                return new ResendOtpResponseDto { IsSuccess = false, Message = "Failed to send OTP. Please try again." };
            }
        }
    }
}