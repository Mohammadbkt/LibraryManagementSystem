using library.Dtos.User;
using library.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using library.Models.Entities;
using library.Dtos.Circulation.User;

namespace library.Controllers
{
    [ApiController]
    [Route("api/users")]
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly UserManager<User> _userManager;

        public UserController(IUserService userService, UserManager<User> userManager)
        {
            _userService = userService;
            _userManager = userManager;
        }

        [HttpGet("me")]
        public async Task<IActionResult> GetProfileAsync()
        {
            var userId = _userManager.GetUserId(User);
            if (userId == null) return Unauthorized();
            var result = await _userService.GetProfileAsync(userId);
            return result != null ? Ok(result) : NotFound();
        }

        [HttpPut("me")]
        public async Task<IActionResult> UpdateProfileAsync([FromBody] UpdateProfileDto dto)
        {
            var userId = _userManager.GetUserId(User);
            if (userId == null) return Unauthorized();
            var result = await _userService.UpdateProfileAsync(userId, dto);
            return Ok(result);
        }

        [HttpDelete("me")]
        public async Task<IActionResult> DeactivateAccountAsync()
        {
            var userId = _userManager.GetUserId(User);
            if (userId == null) return Unauthorized();
            await _userService.DeactivateAccountAsync(userId);
            return Ok(new { message = "Account deactivated successfully" });
        }

        [HttpGet("me/sessions")]
        public async Task<IActionResult> GetActiveSessionsAsync()
        {
            var userId = _userManager.GetUserId(User);
            if (userId == null) return Unauthorized();
            var result = await _userService.GetActiveSessionsAsync(userId);
            return Ok(result);
        }

        [HttpDelete("me/sessions/{sessionId}")]
        public async Task<IActionResult> RevokeSessionAsync(int sessionId)
        {
            var userId = _userManager.GetUserId(User);
            if (userId == null) return Unauthorized();
            await _userService.RevokeSessionAsync(userId, sessionId);
            return Ok(new { message = "Session revoked successfully" });
        }

        // ─── Admin ────────────────────────────────────────────────────────────

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllUsersAsync([FromQuery] UserQueryParams queryParams)
        {
            var result = await _userService.GetAllUsersAsync(queryParams);
            return Ok(result);
        }

        [HttpPost("{id}/deactivate")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeactivateUserAsync(string id)
        {
            var adminId = _userManager.GetUserId(User);
            if (adminId == null) return Unauthorized();
            await _userService.DeactivateUserAsync(adminId, id);
            return Ok(new { message = "User deactivated successfully" });
        }

        [HttpPost("{id}/activate")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ActivateUserAsync(string id)
        {
            var adminId = _userManager.GetUserId(User);
            if (adminId == null) return Unauthorized();
            await _userService.ActivateUserAsync(adminId, id);
            return Ok(new { message = "User activated successfully" });
        }
    }
}