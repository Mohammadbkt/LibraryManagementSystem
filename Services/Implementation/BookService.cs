using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using library.Data;
using library.Dtos.Catalog.Book;
using library.Services.Interface;

namespace library.Services.Implementation
{
    public class BookService : IBookService
    {

        private readonly AppDbContext _context;

        public BookService(AppDbContext context)
        {
            _context = context;
        }

        public Task<BookResponseDto> CreateBookAsync(BookCreateDto dto)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteBookAsync(string id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<BookSummaryDto>> GetAllBooksAsync()
        {
            throw new NotImplementedException();
        }

        public Task<BookDetailDto?> GetBookByIdAsync(string id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<BookSummaryDto>> GetBooksByAuthorAsync(int authorId)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<BookSummaryDto>> GetBooksByCategoryAsync(int categoryId)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<BookSummaryDto>> GetBooksByPublisherAsync(int publisherId)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<BookSummaryDto>> SearchBooksAsync(string query)
        {
            throw new NotImplementedException();
        }

        public Task<BookResponseDto> UpdateBookAsync(string id, BookUpdateDto dto)
        {
            throw new NotImplementedException();
        }
    }
}