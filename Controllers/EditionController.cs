using library.Dtos.Catalog.Edition;
using library.Dtos.Catalog.Item;
using library.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace library.Controllers
{
    [ApiController]
    [Route("api/editions")]
    public class EditionController : ControllerBase
    {
        private readonly IEditionService _editionService;

        public EditionController(IEditionService editionService)
        {
            _editionService = editionService;
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetEditionByIdAsync(int id)
        {
            var result = await _editionService.GetEditionByIdAsync(id);
            return result != null ? Ok(result) : NotFound();
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateEditionAsync([FromBody] EditionCreateDto dto)
        {
            var result = await _editionService.CreateEditionAsync(dto);
            return CreatedAtAction(nameof(GetEditionByIdAsync), new { id = result.Id }, result);
        }

        [HttpPut("{id:int}")]
        [Authorize(Roles = "Admin,Librarian")]
        public async Task<IActionResult> UpdateEditionAsync(int id, [FromBody] EditionUpdateDto dto)
        {
            var result = await _editionService.UpdateEditionAsync(id, dto);
            return Ok(result);
        }

        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteEditionAsync(int id)
        {
            await _editionService.DeleteEditionAsync(id);
            return NoContent();
        }

        [HttpGet("{id:int}/items")]
        public async Task<IActionResult> GetItemsByEditionAsync(int id, [FromQuery] ItemQueryParams queryParams)
        {
            var result = await _editionService.GetItemsByEditionAsync(id, queryParams);
            return Ok(result);
        }

        [HttpGet("{id:int}/items/available")]
        public async Task<IActionResult> GetAvailableItemsAsync(int id, [FromQuery] ItemQueryParams queryParams)
        {
            var result = await _editionService.GetAvailableItemsAsync(id, queryParams);
            return Ok(result);
        }
    }
}