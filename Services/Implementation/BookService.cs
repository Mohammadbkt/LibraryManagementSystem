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
        private const int MAX_AUTHORS = 20;

        public BookService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<BookResponseDto> CreateBookAsync(BookCreateDto dto)
        {
            if (dto.AuthorIds.Count() > MAX_AUTHORS)
                throw new InvalidOperationException($"Cannot have more than {MAX_AUTHORS} authors per book.");

            var publisherExists = await _context.Publishers.AsNoTracking()
                .AnyAsync(p => p.Id == dto.PublisherId);

            if (!publisherExists)
                throw new KeyNotFoundException("Publisher not found");

            var duplicateBook = await _context.Books
                .AnyAsync(b => b.Title == dto.Title && b.PublisherId == dto.PublisherId);

            if (duplicateBook)
                throw new InvalidOperationException("A book with this title already exists for this publisher");

            var authorCount = await _context.Authors
                .CountAsync(a => dto.AuthorIds.Contains(a.Id));

            if (authorCount != dto.AuthorIds.Count())
                throw new KeyNotFoundException("One or more authors not found.");

            var categoryCount = await _context.Categories
                .CountAsync(c => dto.CategoryIds.Contains(c.Id));

            if (categoryCount != dto.CategoryIds.Count())
                throw new KeyNotFoundException("One or more categories not found.");

            var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
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
                await transaction.CommitAsync();

                var createdBook = await _context.Books
                                        .AsNoTracking()
                                        .Where(b => b.Id == book.Id)
                                        .Select(BookMappers.ToDetailDto())
                                        .FirstOrDefaultAsync();

                return new BookResponseDto()
                {
                    IsSuccess = true,
                    Message = "",
                    Book = createdBook ?? throw new Exception("Failed to load created book")
                };
            }
            catch (System.Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }

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
                            .Select(BookMappers.ToSummaryDto())
                            .ToListAsync();

            return new PagedResult<BookSummaryDto>
            {
                Items = books,
                TotalCount = totalCount,
                Page = queryParams.Page,
                PageSize = queryParams.PageSize
            };
        }

        public async Task<BookDetailDto?> GetBookByIdAsync(int id)
        {
            return await _context.Books
                            .AsNoTracking()
                            .Where(b => b.Id == id)
                            .Select(BookMappers.ToDetailDto())
                            .FirstOrDefaultAsync();
        }

        public async Task<PagedResult<BookSummaryDto>> GetBooksByAuthorAsync(int authorId, BookQueryParams queryParams)
        {
            var authorExists = await _context.Authors.AsNoTracking().AnyAsync(a => a.Id == authorId);
            if (!authorExists)
                throw new KeyNotFoundException("Author does not exists");

            var bookQuery = _context.Books
                                    .AsNoTracking()
                                    .Where(b => b.BookAuthors.Any(ba => ba.AuthorId == authorId));

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
                            .Select(BookMappers.ToSummaryDto())
                            .ToListAsync();

            return new PagedResult<BookSummaryDto>
            {
                Items = books,
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
                                        .Where(b => b.BookCategories.Any(bc => bc.CategoryId == categoryId));

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
                            .Select(BookMappers.ToSummaryDto())
                            .ToListAsync();

            return new PagedResult<BookSummaryDto>
            {
                Items = books,
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
                                    .Where(b => b.PublisherId == publisherId);

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
                            .Select(BookMappers.ToSummaryDto())
                            .ToListAsync();

            return new PagedResult<BookSummaryDto>
            {
                Items = books,
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
                                    .FirstOrDefaultAsync(b => b.Id == id);

            if (book == null)
                throw new KeyNotFoundException("Book not found");

            var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
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
                        .AnyAsync(p => p.Id == dto.PublisherId);
                    if (!publisherExists)
                        throw new KeyNotFoundException("Publisher not found");
                }

                if (dto.AuthorIds != null)
                {
                    var authors = await _context.Authors
                        .Where(a => dto.AuthorIds.Contains(a.Id))
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
                        .Where(c => dto.CategoryIds.Contains(c.Id))
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

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                var bookToReturn = await _context.Books
                                        .AsNoTracking()
                                        .Where(b => b.Id == id)
                                        .Select(BookMappers.ToDetailDto())
                                        .FirstOrDefaultAsync();

                if (bookToReturn == null)
                    throw new Exception("Failed to retrieve updated book");

                return bookToReturn;

            }
            catch (System.Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
    }
}

