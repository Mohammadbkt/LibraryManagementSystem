using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using library.Dtos.Catalog.Book;
using library.Dtos.Catalog.Edition;
using library.Dtos.Catalog.Item;
using library.Dtos.Circulation.Review;
using library.Models.Entities;
using library.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace library.Controllers
{
    [ApiController]
    [Route("api/books")]
    public class BookController : ControllerBase
    {
        private readonly IBookService _bookService;
        private readonly IEditionService _editionService;
        private readonly IUserActivityService _userActivityService;
        private readonly UserManager<User> _userManager;

        public BookController(IBookService bookService, IEditionService editionService, IUserActivityService userActivityService, UserManager<User> userManager)
        {
            _bookService = bookService;
            _editionService = editionService;
            _userActivityService = userActivityService;
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<ActionResult> GetAllBooksAsync([FromQuery] BookQueryParams queryParams)
        {
            var result = await _bookService.GetAllBooksAsync(queryParams);
            return Ok(result);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult> GetBookByIdAsync(int id)
        {
            var result = await _bookService.GetBookByIdAsync(id);
            return result != null ? Ok(result) : NotFound();
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]

        public async Task<ActionResult> CreateBookAsync([FromQuery] BookCreateDto dto)
        {
            var result = await _bookService.CreateBookAsync(dto);
            return CreatedAtAction(nameof(GetBookByIdAsync), new { id = result.Book?.Id }, result);
        }

        [HttpPut("{id:int}")]
        [Authorize(Roles = "Admin")]

        public async Task<ActionResult> UpdateBookAsync(int id, [FromQuery] BookUpdateDto dto)
        {
            var result = await _bookService.UpdateBookAsync(id, dto);
            return Ok(result);
        }

        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> DeleteBookAsync(int id)
        {
            await _bookService.DeleteBookAsync(id);
            return NoContent();
        }

        [HttpGet("{id:int}/editions")]
        public async Task<ActionResult> GetBookEditionsAsync(int id, [FromQuery] EditionQueryParams queryParams)
        {
            var result = await _editionService.GetEditionsByBookAsync(id, queryParams);
            return Ok(result);
        }

        [HttpGet("{id:int}/items")]
        public async Task<ActionResult> GetBookItemsAsync(int id, [FromQuery] ItemQueryParams queryParams)
        {
            var result = await _editionService.GetItemsByBookAsync(id, queryParams);
            return Ok(result);
        }

        [HttpGet("{id:int}/reviews")]
        public async Task<ActionResult> GetBookReviewsAsync(int id, ReviewQueryParam queryParam)
        {
            var userId = _userManager.GetUserId(User);
            if (userId == null)
                return Unauthorized();

            var result = await _userActivityService.GetBookReviewsAsync(userId, id, queryParam);
            return Ok(result);
        }
    }
}