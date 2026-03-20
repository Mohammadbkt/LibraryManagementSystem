using System.Security.Claims;
using library.Dtos.Auth;
using library.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace library.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> RegisterAsync([FromBody] RegisterDto dto)
        {
            var deviceName = HttpContext.Request.Headers.UserAgent.ToString();
            var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "";

            var result = await _authService.RegisterAsync(dto, deviceName, ipAddress);

            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        [HttpPost("login")]
        public async Task<IActionResult> LoginAsync([FromBody] LoginDto dto)
        {
            var deviceName = HttpContext.Request.Headers.UserAgent.ToString();
            var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "";

            var result = await _authService.LoginAsync(dto, deviceName, ipAddress);

            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        [HttpPost("refresh")]
        public async Task<IActionResult> RefreshAsync([FromBody] RefreshRequestDto dto)
        {
            var deviceName = HttpContext.Request.Headers.UserAgent.ToString();
            var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "";

            var result = await _authService.RefreshTokenAsync(dto.AccessToken, dto.RefreshToken, deviceName, ipAddress);

            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        [HttpPost("logout")]
        [Authorize]
        public async Task<IActionResult> LogoutAsync([FromBody] LogoutDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId == null)
                return Unauthorized();

            await _authService.LogoutAsync(userId, dto.RefreshToken);

            return Ok(new { message = "Logged out successfully" });
        }

        [HttpPost("logout-all")]
        [Authorize]
        public async Task<IActionResult> LogoutAllAsync()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId == null)
                return Unauthorized();

            await _authService.LogoutAllDevicesAsync(userId);

            return Ok(new { message = "Logged out from all devices" });
        }

        [HttpPost("change-password")]
        [Authorize]
        public async Task<IActionResult> ChangePasswordAsync([FromBody] ChangePasswordDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId == null)
                return Unauthorized();

            var result = await _authService.ChangePasswordAsync(userId, dto.CurrentPassword, dto.NewPassword);

            return result ? Ok(new { message = "Password changed successfully" }) : BadRequest(new { message = "Current password is incorrect" });
        }

        [HttpPost("verify-otp")]
        public async Task<IActionResult> VerifyOtpAsync([FromBody] VerifyOtpRequestDto dto)
        {
            var deviceName = HttpContext.Request.Headers.UserAgent.ToString();

            var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "";

            var result = await _authService.VerifyOtpAsync(dto.UserId, dto.OtpCode, deviceName, ipAddress);

            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        [HttpPost("resend-otp")]
        public async Task<IActionResult> ResendOtpAsync([FromBody] ResendOtpRequestDto dto)
        {
            var result = await _authService.ResendOtpAsync(dto.UserId);
            
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }
    }
}