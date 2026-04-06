using library.Data;
using library.Dtos.Catalog.Author;
using library.Dtos.Common;
using library.Mappers;
using library.Models.Entities;
using library.Services.Interface;
using Microsoft.EntityFrameworkCore;

namespace library.Services.Implementation
{
    public class AuthorService : IAuthorService
    {
        private readonly AppDbContext _context;

        public AuthorService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<AuthorDto> CreateAuthorAsync(AuthorCreateDto dto)
        {
            var existing = await _context.Authors
                .FirstOrDefaultAsync(a => a.FullName == dto.FullName);

            if (existing != null)
                throw new InvalidOperationException("Author already exists");

            var author = dto.ToEntity();

            await _context.Authors.AddAsync(author);
            await _context.SaveChangesAsync();

            var bookToReturn = await _context.Authors
                            .AsNoTracking()
                            .Where(a => a.Id == author.Id)
                            .Select(AuthorMapper.ToDto())
                            .FirstOrDefaultAsync();
                            
            if (bookToReturn == null)
                throw new Exception("Unexpected error occurred while retrieving created author");

            return bookToReturn;
        }

        public async Task DeleteAuthorAsync(int id)
        {
            var author = await _context.Authors
                .FirstOrDefaultAsync(a => a.Id == id);

            if (author == null)
                throw new KeyNotFoundException("Author not found");

            var hasBooks = await _context.BookAuthors
                .AnyAsync(ba => ba.AuthorId == id);

            if (hasBooks)
                throw new InvalidOperationException("Cannot delete author that has books");

            author.IsDeleted = true;
            author.DeletedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
        }

        public async Task<PagedResult<AuthorDto>> GetAllAuthorsAsync(AuthorQueryParams queryParams)
        {
            var query = _context.Authors.AsQueryable();

            if (!string.IsNullOrWhiteSpace(queryParams.Search))
                query = query.Where(a => a.FullName.Contains(queryParams.Search));

            var totalCount = await query.CountAsync();

            var authors = await query
                .AsNoTracking()
                .OrderBy(a => a.FullName)
                .Skip((queryParams.Page - 1) * queryParams.PageSize)
                .Take(queryParams.PageSize)
                .Select(AuthorMapper.ToDto())
                .ToListAsync();

            return new PagedResult<AuthorDto>
            {
                Items = authors,
                TotalCount = totalCount,
                Page = queryParams.Page,
                PageSize = queryParams.PageSize
            };
        }

        public async Task<AuthorDetailDto?> GetAuthorByIdAsync(int id)
        {
            return await _context.Authors
                            .AsNoTracking()
                            .Where(b => b.Id == id)
                            .Select(AuthorMapper.ToDetailDto())
                            .FirstOrDefaultAsync();
        }

        public async Task<AuthorDetailDto> UpdateAuthorAsync(int id, AuthorUpdateDto dto)
        {
            var author = await _context.Authors
                .FirstOrDefaultAsync(a => a.Id == id);

            if (author == null)
                throw new KeyNotFoundException("Author not found");

            if (dto.FullName != null)
            {
                var nameExists = await _context.Authors
                    .AnyAsync(a => a.FullName == dto.FullName && a.Id != id);

                if (nameExists)
                    throw new InvalidOperationException("Author with this name already exists");
            }

            if (!string.IsNullOrWhiteSpace(dto.FullName))
                author.FullName = dto.FullName;

            if (dto.Bio != null)
                author.Biography = dto.Bio;

            await _context.SaveChangesAsync();

            var authorToReturn = await _context.Authors
                            .AsNoTracking()
                            .Where(b => b.Id == id)
                            .Select(AuthorMapper.ToDetailDto())
                            .FirstOrDefaultAsync();

            if (authorToReturn == null)
                throw new Exception("Unexpected error occurred while retrieving updated author");

            return authorToReturn;
        }

    }
}