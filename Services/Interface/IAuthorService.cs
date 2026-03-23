using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using library.Dtos.Catalog.Author;
using library.Dtos.Common;

namespace library.Services.Interface
{
    public interface IAuthorService
    {
        Task<PagedResult<AuthorDto>> GetAllAuthorsAsync(AuthorQueryParams queryParams);
        Task<AuthorDetailDto?> GetAuthorByIdAsync(int id);
        Task<AuthorDto> CreateAuthorAsync(AuthorCreateDto dto);
        Task<AuthorDetailDto> UpdateAuthorAsync(int id, AuthorUpdateDto dto);
        Task DeleteAuthorAsync(int id);
    }
}