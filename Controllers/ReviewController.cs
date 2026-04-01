using library.Dtos.Circulation.Review;
using library.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using library.Models.Entities;

namespace library.Controllers
{
    [ApiController]
    [Route("api/reviews")]
    [Authorize]
    public class ReviewController : ControllerBase
    {
        private readonly IUserActivityService _userActivityService;
        private readonly UserManager<User> _userManager;

        public ReviewController(
            IUserActivityService userActivityService,
            UserManager<User> userManager)
        {
            _userActivityService = userActivityService;
            _userManager = userManager;
        }

        [HttpGet("my")]
        public async Task<IActionResult> GetMyReviewsAsync(ReviewQueryParam queryParams)
        {
            var userId = _userManager.GetUserId(User);
            if (userId == null) return Unauthorized();
            var result = await _userActivityService.GetUserReviewsAsync(userId, queryParams);
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> AddReviewAsync([FromBody] AddReviewDto dto)
        {
            var userId = _userManager.GetUserId(User);
            if (userId == null) return Unauthorized();
            var result = await _userActivityService.AddReviewAsync(userId, dto);
            return Ok(result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateReviewAsync(int id, [FromBody] UpdateReviewDto dto)
        {
            var userId = _userManager.GetUserId(User);
            if (userId == null) return Unauthorized();
            var result = await _userActivityService.UpdateReviewAsync(userId, id, dto);
            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteReviewAsync(int id)
        {
            var userId = _userManager.GetUserId(User);
            if (userId == null) return Unauthorized();
            await _userActivityService.DeleteReviewAsync(userId, id);
            return NoContent();
        }
    }
}