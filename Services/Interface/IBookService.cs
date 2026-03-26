using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using library.Dtos.Catalog.Book;
using library.Dtos.Common;

namespace library.Services.Interface
{
    public interface IBookService
    {
        Task<PagedResult<BookSummaryDto>> GetAllBooksAsync(BookQueryParams queryParams);
        Task<BookDetailDto?> GetBookByIdAsync(int id);
        Task<BookResponseDto> CreateBookAsync(BookCreateDto dto);
        Task<BookDetailDto> UpdateBookAsync(int id, BookUpdateDto dto);
        Task DeleteBookAsync(int id);
        Task<PagedResult<BookSummaryDto>> GetBooksByAuthorAsync(int authorId, BookQueryParams queryParams);
        Task<PagedResult<BookSummaryDto>> GetBooksByPublisherAsync(int publisherId, BookQueryParams queryParams);
        Task<PagedResult<BookSummaryDto>> GetBooksByCategoryAsync(int categoryId, BookQueryParams queryParams);
    }
}