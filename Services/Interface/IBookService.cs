using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using library.Dtos.Catalog.Book;

namespace library.Services.Interface
{
    public interface IBookService
    {
        Task<IEnumerable<BookSummaryDto>> GetAllBooksAsync();
        Task<BookDetailDto?> GetBookByIdAsync(string id);
        Task<BookResponseDto> CreateBookAsync(BookCreateDto dto);
        Task<BookResponseDto> UpdateBookAsync(string id, BookUpdateDto dto);
        Task<bool> DeleteBookAsync(string id);
        Task<IEnumerable<BookSummaryDto>> SearchBooksAsync(string query);
        Task<IEnumerable<BookSummaryDto>> GetBooksByAuthorAsync(int authorId);
        Task<IEnumerable<BookSummaryDto>> GetBooksByPublisherAsync(int publisherId);
        Task<IEnumerable<BookSummaryDto>> GetBooksByCategoryAsync(int categoryId);
    }
}