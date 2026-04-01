using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using library.Dtos.Catalog.Author;
using library.Dtos.Common;
using library.Services.Interface;
using Microsoft.AspNetCore.Mvc;

namespace library.Controllers
{
    [ApiController]
    [Route("api/authors")]
    public class AuthorController : ControllerBase
    {
        private readonly IAuthorService _authorService;

        public AuthorController(IAuthorService authorService)
        {
            _authorService = authorService;
        }

        [HttpGet]
        public async Task<ActionResult<PagedResult<AuthorDto>>> GetAllAuthors([FromQuery] AuthorQueryParams queryParams)
        {
            var result = await _authorService.GetAllAuthorsAsync(queryParams);
            return Ok(result);
        }
        
        [HttpGet("{id:int}")]
        public async Task<ActionResult<AuthorDto>> GetAuthorById(int id)
        {
            var result = await _authorService.GetAuthorByIdAsync(id);
            if (result == null)
                return NotFound();
            return Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult<AuthorDto>> CreateAuthor([FromBody] AuthorCreateDto dto)
        {
            var result = await _authorService.CreateAuthorAsync(dto);
            return CreatedAtAction(nameof(GetAuthorById), new { id = result.Id }, result);
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult> UpdateAuthor(int id, [FromBody] AuthorUpdateDto dto)
        {
            var success = await _authorService.UpdateAuthorAsync(id, dto);
            if (success == null)
                return NotFound();
            return Ok(success);
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> DeleteAuthor(int id)
        {
            await _authorService.DeleteAuthorAsync(id);
            return NoContent();
        }
    }
}