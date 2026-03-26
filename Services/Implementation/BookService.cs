using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using library.Data;
using library.Dtos.Catalog.Book;
using library.Dtos.Common;
using library.Mappers;
using library.Models.Entities;
using library.Services.Interface;
using Microsoft.EntityFrameworkCore;

namespace library.Services.Implementation
{
    public class BookService : IBookService
    {

        private readonly AppDbContext _context;

        public BookService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<BookResponseDto> CreateBookAsync(BookCreateDto dto)
        {
            var publisherExists = await _context.Publishers.AsNoTracking()
                .AnyAsync(p => p.Id == dto.PublisherId);

            if (!publisherExists)
                throw new KeyNotFoundException("Publisher not found");

            var duplicateBook = await _context.Books
                .AnyAsync(b => b.Title == dto.Title && b.PublisherId == dto.PublisherId);

            if (duplicateBook)
                throw new InvalidOperationException("A book with this title already exists for this publisher");

            var authors = await _context.Authors
                                .Where(a => dto.AuthorIds.Contains(a.Id))
                                .ToListAsync();

            if (authors.Count != dto.AuthorIds.Count())
                throw new KeyNotFoundException("One or more authors not found");

            var categories = await _context.Categories
                                .Where(c => dto.CategoryIds.Contains(c.Id))
                                .ToListAsync();

            if (categories.Count != dto.CategoryIds.Count())
                throw new KeyNotFoundException("One or more categories not found");


            var book = dto.ToEntity();

            book.BookAuthors = dto.AuthorIds
                .Select((authorId, index) => new BookAuthor
                {
                    AuthorId = authorId,
                    AuthorOrder = index + 1
                }).ToList();

            book.BookCategories = dto.CategoryIds
                .Select(categoryId => new BookCategory
                {
                    CategoryId = categoryId
                }).ToList();

            await _context.Books.AddAsync(book);
            await _context.SaveChangesAsync();

            return new BookResponseDto()
            {
                IsSuccess = true,
                Message = "",
                Book = book.ToDetailDto()
            };
        }

        public async Task DeleteBookAsync(int id)
        {
            var bookExists = await _context.Books.FindAsync(id);
            if (bookExists == null)
                throw new KeyNotFoundException("book does not exists");

            var hasActiveLoans = await _context.Loans
                        .AnyAsync(l => l.Item.Edition.BookId == id && l.ReturnDate == null);

            if (hasActiveLoans)
                throw new InvalidOperationException("Cannot delete book that has active loans");

            bookExists.IsDeleted = true;
            bookExists.DeletedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
        }

        public async Task<PagedResult<BookSummaryDto>> GetAllBooksAsync(BookQueryParams queryParams)
        {
            var bookQuery = _context.Books
                                .AsNoTracking()
                                .Include(b => b.BookAuthors)
                                    .ThenInclude(ba => ba.Author)
                                .Include(b => b.BookCategories)
                                    .ThenInclude(bc => bc.Category)
                                .Include(b => b.Publisher)
                                .Include(b => b.Editions)
                                    .ThenInclude(e => e.Items)
                                .Where(b => !b.IsDeleted)
                                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(queryParams.Search))
            {
                bookQuery = bookQuery.Where(b =>
                    b.Title.Contains(queryParams.Search) ||
                    b.BookAuthors.Any(ba =>
                        ba.Author.FullName.Contains(queryParams.Search)));
            }

            var totalCount = await bookQuery.CountAsync();

            var books = await bookQuery
                            .OrderBy(b => b.Title)
                            .Skip((queryParams.Page - 1) * queryParams.PageSize)
                            .Take(queryParams.PageSize)
                            .ToListAsync();

            var bookSummaryDtos = books.Select(b => b.ToSummaryDto()).ToList();

            return new PagedResult<BookSummaryDto>
            {
                Items = bookSummaryDtos,
                TotalCount = totalCount,
                Page = queryParams.Page,
                PageSize = queryParams.PageSize
            };
        }

        public async Task<BookDetailDto?> GetBookByIdAsync(int id)
        {
            var bookExists = await _context.Books
                                            .AsNoTracking()
                                            .Include(b => b.BookAuthors)
                                                .ThenInclude(ba => ba.Author)
                                            .Include(b => b.BookCategories)
                                                .ThenInclude(bc => bc.Category)
                                            .Include(b => b.Publisher)
                                            .Include(b => b.Editions)
                                            .FirstOrDefaultAsync(b => b.Id == id && !b.IsDeleted);

            if (bookExists == null)
                return null;

            return bookExists.ToDetailDto();
        }

        public async Task<PagedResult<BookSummaryDto>> GetBooksByAuthorAsync(int authorId, BookQueryParams queryParams)
        {
            var authorExists = await _context.Authors.AsNoTracking().AnyAsync(a => a.Id == authorId);
            if (!authorExists)
                throw new KeyNotFoundException("Author does not exists");

            var bookQuery = _context.Books
                                    .AsNoTracking()
                                    .Include(b => b.BookAuthors)
                                        .ThenInclude(ba => ba.Author)
                                    .Include(b => b.BookCategories)
                                        .ThenInclude(bc => bc.Category)
                                    .Include(b => b.Publisher)
                                    .Include(b => b.Editions)
                                        .ThenInclude(e => e.Items)
                                    .Where(b => b.BookAuthors.Any(ba => ba.AuthorId == authorId) && !b.IsDeleted);

            if (!string.IsNullOrWhiteSpace(queryParams.Search))
            {
                bookQuery = bookQuery.Where(b =>
                    b.Title.Contains(queryParams.Search) ||
                    b.BookAuthors.Any(ba =>
                        ba.Author.FullName.Contains(queryParams.Search)));
            }

            var totalCount = await bookQuery.CountAsync();

            var books = await bookQuery
                            .OrderBy(b => b.Title)
                            .Skip((queryParams.Page - 1) * queryParams.PageSize)
                            .Take(queryParams.PageSize)
                            .ToListAsync();

            return new PagedResult<BookSummaryDto>
            {
                Items = books.Select(b => b.ToSummaryDto()).ToList(),
                TotalCount = totalCount,
                Page = queryParams.Page,
                PageSize = queryParams.PageSize
            };

        }

        public async Task<PagedResult<BookSummaryDto>> GetBooksByCategoryAsync(int categoryId, BookQueryParams queryParams)
        {
            var categoryExists = await _context.Categories.AsNoTracking().AnyAsync(c => c.Id == categoryId);
            if (!categoryExists)
                throw new KeyNotFoundException($"There is no Category with the {categoryId} Id");

            var bookQuery = _context.Books
                                        .AsNoTracking()
                                        .Include(b => b.BookAuthors)
                                        .Include(b => b.BookCategories)
                                            .ThenInclude(bc => bc.Category)
                                        .Include(b => b.Publisher)
                                        .Include(b => b.Editions)
                                            .ThenInclude(e => e.Items)
                                        .Where(b => b.BookCategories.Any(bc => bc.CategoryId == categoryId) && !b.IsDeleted);

            if (!string.IsNullOrWhiteSpace(queryParams.Search))
            {
                bookQuery = bookQuery.Where(b =>
                    b.Title.Contains(queryParams.Search) ||
                    b.BookAuthors.Any(ba =>
                        ba.Author.FullName.Contains(queryParams.Search)));
            }


            var totalCount = await bookQuery.CountAsync();

            var books = await bookQuery
                            .OrderBy(b => b.Title)
                            .Skip((queryParams.Page - 1) * queryParams.PageSize)
                            .Take(queryParams.PageSize)
                            .ToListAsync();

            return new PagedResult<BookSummaryDto>
            {
                Items = books.Select(b => b.ToSummaryDto()).ToList(),
                TotalCount = totalCount,
                Page = queryParams.Page,
                PageSize = queryParams.PageSize
            };

        }

        public async Task<PagedResult<BookSummaryDto>> GetBooksByPublisherAsync(int publisherId, BookQueryParams queryParams)
        {
            var publisherExists = await _context.Publishers.AsNoTracking().AnyAsync(p => p.Id == publisherId);
            if (!publisherExists)
                throw new KeyNotFoundException($"There is no Publisher With the {publisherId} Id");

            var bookQuery = _context.Books
                                    .AsNoTracking()
                                    .Include(b => b.BookAuthors)
                                    .Include(b => b.BookCategories)
                                        .ThenInclude(bc => bc.Category)
                                    .Include(b => b.Publisher)
                                    .Include(b => b.Editions)
                                        .ThenInclude(e => e.Items)
                                    .Where(b => b.PublisherId == publisherId && !b.IsDeleted);

            if (!string.IsNullOrWhiteSpace(queryParams.Search))
            {
                bookQuery = bookQuery.Where(b =>
                    b.Title.Contains(queryParams.Search) ||
                    b.BookAuthors.Any(ba =>
                        ba.Author.FullName.Contains(queryParams.Search)));
            }


            var totalCount = await bookQuery.CountAsync();

            var books = await bookQuery
                            .OrderBy(b => b.Title)
                            .Skip((queryParams.Page - 1) * queryParams.PageSize)
                            .Take(queryParams.PageSize)
                            .ToListAsync();

            return new PagedResult<BookSummaryDto>
            {
                Items = books.Select(b => b.ToSummaryDto()).ToList(),
                TotalCount = totalCount,
                Page = queryParams.Page,
                PageSize = queryParams.PageSize
            };

        }
        public async Task<BookDetailDto> UpdateBookAsync(int id, BookUpdateDto dto)
        {
            var book = await _context.Books
                                    .Include(b => b.BookAuthors)
                                    .Include(b => b.BookCategories)
                                    .FirstOrDefaultAsync(b => b.Id == id && !b.IsDeleted);

            if (book == null)
                throw new KeyNotFoundException("Book not found");

            if (dto.Title != null)
            {
                var duplicate = await _context.Books
                    .AnyAsync(b => b.Title == dto.Title && b.PublisherId == (dto.PublisherId ?? book.PublisherId) && b.Id != id);
                if (duplicate)
                    throw new InvalidOperationException("A book with this title already exists for this publisher");
            }

            if (dto.PublisherId.HasValue)
            {
                var publisherExists = await _context.Publishers
                    .AnyAsync(p => p.Id == dto.PublisherId && !p.IsDeleted);
                if (!publisherExists)
                    throw new KeyNotFoundException("Publisher not found");
            }

            if (dto.AuthorIds != null)
            {
                var authors = await _context.Authors
                    .Where(a => dto.AuthorIds.Contains(a.Id) && !a.IsDeleted)
                    .ToListAsync();
                if (authors.Count != dto.AuthorIds.Count())
                    throw new KeyNotFoundException("One or more authors not found");

                if (dto.AuthorIds.Distinct().Count() != dto.AuthorIds.Count())
                    throw new InvalidOperationException("Duplicate author IDs are not allowed");

                book.BookAuthors.Clear();
                book.BookAuthors = dto.AuthorIds
                    .Select((authorId, index) => new BookAuthor
                    {
                        BookId = id,
                        AuthorId = authorId,
                        AuthorOrder = index + 1
                    }).ToList();
            }

            if (dto.CategoryIds != null)
            {
                var categories = await _context.Categories
                    .Where(c => dto.CategoryIds.Contains(c.Id) && !c.IsDeleted)
                    .ToListAsync();
                if (categories.Count != dto.CategoryIds.Count())
                    throw new KeyNotFoundException("One or more categories not found");

                book.BookCategories.Clear();
                book.BookCategories = dto.CategoryIds
                    .Select(categoryId => new BookCategory
                    {
                        BookId = id,
                        CategoryId = categoryId
                    }).ToList();
            }

            dto.ApplyUpdate(book);
            await _context.SaveChangesAsync();

            var bookToReturn = await _context.Books
                                    .Include(b => b.Publisher)
                                    .Include(b => b.BookAuthors.OrderBy(ba => ba.AuthorOrder))
                                        .ThenInclude(ba => ba.Author)
                                    .Include(b => b.BookCategories)
                                        .ThenInclude(bc => bc.Category)
                                    .Include(b => b.Editions.Where(e => !e.IsDeleted))
                                        .ThenInclude(e => e.Items.Where(i => !i.IsDeleted))
                                    .AsNoTracking()
                                    .FirstOrDefaultAsync(b => b.Id == id);

            if (bookToReturn == null)
                throw new Exception("Failed to retrieve updated book");

            return bookToReturn.ToDetailDto();

        }


    }
}