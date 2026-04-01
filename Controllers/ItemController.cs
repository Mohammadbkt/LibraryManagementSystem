using library.Dtos.Catalog.Item;
using library.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace library.Controllers
{
    [ApiController]
    [Route("api/items")]
    public class ItemController : ControllerBase
    {
        private readonly IEditionService _editionService;

        public ItemController(IEditionService editionService)
        {
            _editionService = editionService;
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetItemByIdAsync(int id)
        {
            var result = await _editionService.GetItemByIdAsync(id);
            return result != null ? Ok(result) : NotFound();
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateItemAsync([FromBody] ItemCreateDto dto)
        {
            var result = await _editionService.CreateItemAsync(dto);
            return CreatedAtAction(nameof(GetItemByIdAsync), new { id = result.Id }, result);
        }

        [HttpPut("{id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateItemAsync(int id, [FromBody] ItemUpdateDto dto)
        {
            var result = await _editionService.UpdateItemAsync(id, dto);
            return Ok(result);
        }

        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteItemAsync(int id)
        {
            await _editionService.DeleteItemAsync(id);
            return NoContent();
        }
    }
}