using library.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using library.Models.Entities;
using library.Dtos.Circulation.Bookmark;

namespace library.Controllers
{
    [ApiController]
    [Route("api/bookmarks")]
    [Authorize]
    public class BookmarkController : ControllerBase
    {
        private readonly IUserActivityService _userActivityService;
        private readonly UserManager<User> _userManager;

        public BookmarkController(
            IUserActivityService userActivityService,
            UserManager<User> userManager)
        {
            _userActivityService = userActivityService;
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> GetUserBookmarksAsync(BookmarkQueryParam queryParams)
        {
            var userId = _userManager.GetUserId(User);
            if (userId == null) return Unauthorized();
            var result = await _userActivityService.GetUserBookmarksAsync(userId, queryParams);
            return Ok(result);
        }

        [HttpPost("{bookId}")]
        public async Task<IActionResult> AddBookmarkAsync(AddBookmarkDto dto, [FromBody] string? notes)
        {
            var userId = _userManager.GetUserId(User);
            if (userId == null) return Unauthorized();
            await _userActivityService.AddBookmarkAsync(userId, dto);
            return Ok(new { message = "Bookmark added successfully" });
        }

        [HttpDelete("{bookId}")]
        public async Task<IActionResult> RemoveBookmarkAsync(int bookId)
        {
            var userId = _userManager.GetUserId(User);
            if (userId == null) return Unauthorized();
            await _userActivityService.RemoveBookmarkAsync(userId, bookId);
            return NoContent();
        }

        [HttpGet("{bookId}")]
        public async Task<IActionResult> IsBookmarkedAsync(int bookId)
        {
            var userId = _userManager.GetUserId(User);
            if (userId == null) return Unauthorized();
            var result = await _userActivityService.IsBookmarkedAsync(userId, bookId);
            return Ok(new { isBookmarked = result });
        }
    }
}