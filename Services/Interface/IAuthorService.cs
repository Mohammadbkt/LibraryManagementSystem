using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using library.Dtos.Catalog.Author;

namespace library.Services.Interface
{
    public interface IAuthorService
    {
        Task<IEnumerable<AuthorDto>> GetAllAuthorsAsync();
    Task<AuthorDetailDto?> GetAuthorByIdAsync(int id);
    Task<AuthorDto> CreateAuthorAsync(AuthorCreateDto dto);
    Task<AuthorDto> UpdateAuthorAsync(int id, AuthorUpdateDto dto);
    Task<bool> DeleteAuthorAsync(int id);
    }
}