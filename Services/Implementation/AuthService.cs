// using System;
// using System.Collections.Generic;
// using System.Linq;
// using System.Threading.Tasks;
// using library.Data;
// using library.Dtos.Auth;
// using library.Models.Entities;
// using library.Services.Interface;
// using Microsoft.AspNetCore.Identity;

// namespace library.Services.Implementation
// {
//     public class AuthService : IAuthService
//     {

//         private readonly AppDbContext _context;
//         private readonly UserManager<User> _userManager;

//         public AuthService(AppDbContext context, UserManager<User> userManager)
//         {
//             _context = context;
//             _userManager = userManager;
//         }

//         public Task<bool> ChangePasswordAsync(string userId, string currentPassword, string newPassword)
//         {
//             throw new NotImplementedException();
//         }

//         public async Task<AuthResponseDto?> LoginAsync(LoginDto loginDto, string deviceName, string ipAddress)
//         {
//             var user = await _userManager.FindByEmailAsync(loginDto.Email);
//             if (user == null)
//             {
//                 return new AuthResponseDto
//                 {
//                     IsSuccess = false,
//                     ExpiresAt = DateTime.UtcNow,
//                 };
//             }

//             if (!user.EmailConfirmed)
//             {
//                 return new AuthResponseDto
//                 {
//                     IsSuccess = false,
//                     ExpiresAt = DateTime.UtcNow,
//                 };
//             }

//             if (!user.IsActive)
//             {
//                 return new AuthResponseDto
//                 {
//                     IsSuccess = false,
//                     ExpiresAt = DateTime.UtcNow,
//                 };
//             }

//             var result = await _signInManager.CheckPasswordSignInAsync(user, loginDto.Password, false);
//             if (!result.Succeeded)
//             {
//                 return new AuthResponseDto
//                 {
//                     IsSuccess = false,
//                     ExpiresAt = DateTime.UtcNow,
//                 };
//             }

//             var roles = await _userManager.GetRolesAsync(user);
//             var (token, expires) = _JwtService.GenerateTokenAsync(user.Id, user.UserName ?? string.Empty, roles.ToList());

//             return new AuthResponseDto
//             {
//                 Success = true,
//                 Token = token,
//                 UserId = user.Id,
//                 UserName = user.UserName ?? string.Empty,
//                 Email = user.Email,
//                 Expiration = expires,
//                 Role = roles.ToList()
//             };
//         }

//         public Task<bool> LogoutAllDevicesAsync(string userId)
//         {
//             throw new NotImplementedException();
//         }

//         public Task<bool> LogoutAsync(string userId, string refreshToken)
//         {
//             throw new NotImplementedException();
//         }

//         public Task<AuthResponseDto?> RefreshTokenAsync(string accessToken, string refreshToken, string deviceName, string ipAddress)
//         {
//             throw new NotImplementedException();
//         }

//         public async Task<AuthResponseDto?> RegisterAsync(RegisterDto registerDto, string deviceName, string ipAddress)
//         {
//             var existingEmail = await _userManager.FindByEmailAsync(registerDto.Email);
//             if (existingEmail != null)
//             {
//                 // if (!existingEmail.EmailConfirmed)
//                 // {
//                 //     try
//                 //     {
//                 //         var otp = await _otpService.GenerateOtpAsync(existingEmail.Id);
//                 //         await _emailService.SendEmailAsync(
//                 //             existingEmail.Email!, 
//                 //             "OTP Verification", 
//                 //             $@"Hello {existingEmail.FullName},
//                 //             Your verification code is: {otp}                            
//                 //             This code will expire in 15 minutes."
//                 //         );

//                 //         return new AuthResponseDto
//                 //         {
//                 //             Success = false,
//                 //             Errors = "Account exists but not verified. New OTP sent to your email."
//                 //         };
//                 //     }
//                 //     catch (Exception)
//                 //     {
//                 //         return new AuthResponseDto
//                 //         {
//                 //             Success = false,
//                 //             Errors = "Failed to send verification email. Please try again."
//                 //         };
//                 //     }
//                 // }

//                 return new AuthResponseDto
//                 {
//                     IsSuccess = false,
//                     ExpiresAt = DateTime.UtcNow
//                 };
//             }

//             var existingUserName = await _userManager.FindByNameAsync(registerDto.UserName);
//             if (existingUserName != null)
//             {
//                 return new AuthResponseDto
//                 {
//                     IsSuccess = false,
//                     ExpiresAt = DateTime.UtcNow
//                 };
//             }

//             var user = new User
//             {
//                 UserName = registerDto.UserName,
//                 Email = registerDto.Email,
//                 PhoneNumber = registerDto.PhoneNumber,
//                 IsActive = false,
//                 EmailConfirmed = false
//             };

//             var result = await _userManager.CreateAsync(user, registerDto.PasswordHash);
//             if (!result.Succeeded)
//             {
//                 return new AuthResponseDto
//                 {
//                     IsSuccess = false,
//                     ExpiresAt = DateTime.UtcNow,
//                 };
//             }

//             // Assign default role
//             await _userManager.AddToRoleAsync(user, "User");

//             // Generate and send OTP
//             // try
//             // {
//             //     var otp = await _otpService.GenerateOtpAsync(user.Id);
//             //     await _emailService.SendEmailAsync(
//             //         user.Email, 
//             //         "OTP Verification",
//             //         $@"Hello {user.FullName},
//             //         Welcome to HRMS! Your verification code is: {otp}
//             //         This code will expire in 15 minutes."
//             //     );
//             // }
//             // catch (Exception)
//             // {
//             //     await _userManager.DeleteAsync(user);

//             //     return new AuthResponseDto
//             //     {
//             //         Success = false,
//             //         Errors = "Failed to send verification email. Please try again."
//             //     };
//             // }

//             return new AuthResponseDto
//             {
//                 IsSuccess = true,
//                 UserId = user.Id,
//                 Email = user.Email,
//                 Roles = ["User"]
//             };
//         }
//     }
// }