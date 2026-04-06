using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using library.Data;
using library.Dtos.Catalog.Edition;
using library.Dtos.Catalog.Item;
using library.Dtos.Common;
using library.Mappers;
using library.Models.Enums;
using library.Services.Interface;
using Microsoft.EntityFrameworkCore;

namespace library.Services.Implementation
{
    public class EditionService : IEditionService
    {

        private readonly AppDbContext _context;

        public EditionService(AppDbContext context)
        {
            _context = context;
        }


        public async Task<EditionDto> CreateEditionAsync(EditionCreateDto dto)
        {
            var bookExists = await _context.Books.AnyAsync(b => b.Id == dto.BookId);
            if (!bookExists)
                throw new KeyNotFoundException("Book not found");

            var edition = dto.ToEntity();

            await _context.Editions.AddAsync(edition);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex) when (ex.InnerException != null)
            {
                var message = ex.InnerException.Message;

                if (message.Contains("IX_Editions_ISBN"))
                    throw new InvalidOperationException("ISBN already exists");

                if (message.Contains("IX_Editions_BookId_EditionNumber"))
                    throw new InvalidOperationException(
                        $"Edition {dto.EditionNumber} already exists for this book");

                throw;
            }

            return await _context.Editions
                .AsNoTracking()
                .Where(e => e.Id == edition.Id)
                .Select(EditionMappers.ToDto())
                .FirstAsync();
        }

        public async Task<ItemDto> CreateItemAsync(ItemCreateDto dto)
        {
            var editionExists = await _context.Editions.AnyAsync(e => e.Id == dto.EditionId);
            if (!editionExists)
                throw new KeyNotFoundException("Edition not found");

            var item = dto.ToEntity();

            await _context.Items.AddAsync(item);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                if (ex.InnerException?.Message.Contains("IX_Items_Barcode") == true)
                    throw new InvalidOperationException("Barcode already exists");

                throw;
            }

            return await _context.Items
                .AsNoTracking()
                .Where(i => i.Id == item.Id)
                .Select(ItemMappers.ToDto())
                .FirstAsync();
        }

        public async Task DeleteEditionAsync(int id)
        {
            var edition = await _context.Editions.FirstOrDefaultAsync(e => e.Id == id);
            if (edition == null)
                throw new KeyNotFoundException($"Edition with id {id} not found");

            var hasItems = await _context.Items.AnyAsync(i => i.EditionId == id);

            if (hasItems)
                throw new InvalidOperationException("Cannot delete edition that has items");

            edition.IsDeleted = true;
            edition.DeletedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
        }

        public async Task DeleteItemAsync(int id)
        {
            var item = await _context.Items.FirstOrDefaultAsync(i => i.Id == id);
            if (item == null)
                throw new KeyNotFoundException("Item not found");

            var isOnLoan = await _context.Loans.AnyAsync(l => l.ItemId == id && l.ReturnDate == null);
            if (isOnLoan)
                throw new InvalidOperationException("Cannot delete item that is currently on loan");

            item.IsDeleted = true;
            item.DeletedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
        }

        public async Task<PagedResult<ItemDto>> GetAvailableItemsAsync(int editionId, ItemQueryParams queryParams)
        {
            var ItemQuery = _context.Items
                                    .AsNoTracking()
                                    .AsQueryable();

            if (!string.IsNullOrWhiteSpace(queryParams.Search))
            {
                ItemQuery = ItemQuery.Where(i => i.Location!.Contains(queryParams.Search));
            }

            var totalCount = await ItemQuery.CountAsync();

            var itemDtos = await ItemQuery
                                    .AsNoTracking()
                                    .Where(i => i.EditionId == editionId)
                                    .OrderBy(i => i.Price)
                                    .Skip((queryParams.Page - 1) * queryParams.PageSize)
                                    .Take(queryParams.PageSize)
                                    .Select(ItemMappers.ToDto())
                                    .ToListAsync();

            return new PagedResult<ItemDto>
            {
                Items = itemDtos,
                TotalCount = totalCount,
                Page = queryParams.Page,
                PageSize = queryParams.PageSize
            };

        }

        public async Task<EditionDto?> GetEditionByIdAsync(int id)
        {
            return await _context.Editions
                            .AsNoTracking()
                            .Where(e => e.Id == id)
                            .Select(EditionMappers.ToDto())
                            .FirstOrDefaultAsync();
        }

        public async Task<PagedResult<EditionDto>> GetEditionsByBookAsync(int bookId, EditionQueryParams queryParams)
        {
            var EditionQuery = _context.Editions
                                    .AsNoTracking()
                                    .AsQueryable();

            if (!string.IsNullOrWhiteSpace(queryParams.Search))
            {
                EditionQuery = EditionQuery.Where(e => e.ISBN.Contains(queryParams.Search));
            }

            var totalCount = await EditionQuery.CountAsync();

            var editionDtos = await EditionQuery
                                    .AsNoTracking()
                                    .Where(e => e.BookId == bookId)
                                    .OrderBy(e => e.EditionNumber)
                                    .Skip((queryParams.Page - 1) * queryParams.PageSize)
                                    .Take(queryParams.PageSize)
                                    .Select(EditionMappers.ToDto())
                                    .ToListAsync();


            return new PagedResult<EditionDto>
            {
                Items = editionDtos,
                TotalCount = totalCount,
                Page = queryParams.Page,
                PageSize = queryParams.PageSize
            };

        }

        public async Task<ItemDto?> GetItemByIdAsync(int id)
        {
            return await _context.Items
                            .AsNoTracking()
                            .Where(i => i.Id == id && !i.IsDeleted)
                            .Select(ItemMappers.ToDto())
                            .FirstOrDefaultAsync();
        }

        public async Task<PagedResult<ItemDto>> GetItemsByBookAsync(int bookId, ItemQueryParams queryParams)
        {

            var bookExists = await _context.Books.AnyAsync(b => b.Id == bookId);
            if (!bookExists)
                throw new KeyNotFoundException($"Book with id {bookId} not found");

            
            var ItemQuery = _context.Items
                                    .AsNoTracking()
                                    .AsQueryable();

            if (!string.IsNullOrWhiteSpace(queryParams.Search))
            {
                ItemQuery = ItemQuery.Where(i => i.Location!.Contains(queryParams.Search));
            }

            var totalCount = await ItemQuery.CountAsync();

            var itemDtos = await ItemQuery
                                .AsNoTracking()
                                .Where(i => i.Edition.BookId == bookId)
                                .OrderBy(i => i.Price)
                                .Skip((queryParams.Page - 1) * queryParams.PageSize)
                                .Take(queryParams.PageSize)
                                .Select(ItemMappers.ToDto())
                                .ToListAsync();



            return new PagedResult<ItemDto>
            {
                Items = itemDtos,
                TotalCount = totalCount,
                Page = queryParams.Page,
                PageSize = queryParams.PageSize
            };
        }

        public async Task<PagedResult<ItemDto>> GetItemsByEditionAsync(int editionId, ItemQueryParams queryParams)
        {
            var ItemQuery = _context.Items
                                    .AsQueryable();

            if (!string.IsNullOrWhiteSpace(queryParams.Search))
            {
                ItemQuery = ItemQuery.Where(i => i.Location != null && i.Location.Contains(queryParams.Search));
            }

            var totalCount = await ItemQuery.CountAsync();

            var itemDtos = await ItemQuery
                                    .AsNoTracking()
                                    .Where(i => i.EditionId == editionId)
                                    .OrderBy(i => i.Price)
                                    .Skip((queryParams.Page - 1) * queryParams.PageSize)
                                    .Take(queryParams.PageSize)
                                    .Select(ItemMappers.ToDto())
                                    .ToListAsync();

            return new PagedResult<ItemDto>
            {
                Items = itemDtos,
                TotalCount = totalCount,
                Page = queryParams.Page,
                PageSize = queryParams.PageSize
            };

        }

        public async Task<EditionDto> UpdateEditionAsync(int id, EditionUpdateDto dto)
        {
            var edition = await _context.Editions
                .FirstOrDefaultAsync(e => e.Id == id);

            if (edition == null)
                throw new KeyNotFoundException("Edition not found");

            if (dto.EditionNumber != null)
                edition.EditionNumber = dto.EditionNumber.Value;

            if (dto.PublicationYear != null)
                edition.PublicationYear = dto.PublicationYear;

            if (dto.ISBN != null)
            {
                var isbnExists = await _context.Editions
                                    .AnyAsync(e => e.ISBN == dto.ISBN && e.Id != id);

                if (isbnExists)
                    throw new InvalidOperationException("ISBN already exists");
            }

            if (dto.CoverImageUrl != null)
                edition.CoverImageUrl = dto.CoverImageUrl;

            if (dto.PageCount != null)
                edition.PageCount = dto.PageCount;

            if (dto.Language != null)
                edition.Language = dto.Language;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                var msg = ex.InnerException?.Message;

                if (msg?.Contains("IX_Editions_ISBN") == true)
                    throw new InvalidOperationException("ISBN already exists");

                if (msg?.Contains("IX_Editions_BookId_EditionNumber") == true)
                    throw new InvalidOperationException("Duplicate edition number");

                throw;
            }

            return await _context.Editions
                .AsNoTracking()
                .Where(e => e.Id == id)
                .Select(EditionMappers.ToDto())
                .FirstAsync();
        }

        public async Task<ItemDto> UpdateItemAsync(int id, ItemUpdateDto dto)
        {
            var item = await _context.Items
                                .FirstOrDefaultAsync(i => i.Id == id);

            if (item == null)
                throw new KeyNotFoundException("Item not found");

            if (dto.Barcode != null)
                item.Barcode = dto.Barcode;

            if (dto.Notes != null)
                item.Notes = dto.Notes;

            if (dto.Location != null)
                item.Location = dto.Location;

            if (dto.Price != null)
                item.Price = dto.Price.Value;

            if (dto.ItemStatus != null)
            {
                if (!Enum.TryParse<ItemStatus>(dto.ItemStatus, true, out var status))
                    throw new ArgumentException("Invalid item status");

                item.ItemStatus = status;
            }

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                var msg = ex.InnerException?.Message;

                if (msg?.Contains("IX_Editions_ISBN") == true)
                    throw new InvalidOperationException("ISBN already exists");

                if (msg?.Contains("IX_Editions_BookId_EditionNumber") == true)
                    throw new InvalidOperationException("Duplicate edition number");

                throw;
            }
            return await _context.Items
                .AsNoTracking()
                .Where(i => i.Id == id)
                .Select(ItemMappers.ToDto())
                .FirstAsync();
        }
    }
}