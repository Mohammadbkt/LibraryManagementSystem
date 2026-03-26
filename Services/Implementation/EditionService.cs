using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using library.Data;
using library.Dtos.Catalog.Edition;
using library.Dtos.Catalog.Item;
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

            var duplicateEdition = await _context.Editions
            .AnyAsync(e => e.BookId == dto.BookId && e.EditionNumber == dto.EditionNumber);

            if (duplicateEdition)
                throw new InvalidOperationException($"Edition {dto.EditionNumber} already exists for this book");


            var isbnExists = await _context.Editions.AnyAsync(e => e.ISBN == dto.ISBN);
            if (isbnExists)
                throw new InvalidOperationException("ISBN already exists");

            var edition = dto.ToEntity();

            await _context.Editions.AddAsync(edition);

            await _context.SaveChangesAsync();

            return edition.ToDto();
        }

        public async Task<ItemDto> CreateItemAsync(ItemCreateDto dto)
        {
            var editionExists = await _context.Editions.AnyAsync(e => e.Id == dto.EditionId);
            if (!editionExists)
                throw new KeyNotFoundException("Edition not found");

            var barcodeExists = await _context.Items
                .AnyAsync(i => i.Barcode == dto.Barcode);

            if (barcodeExists)
                throw new InvalidOperationException("Barcode already exists");

            var item = dto.ToEntity();

            await _context.Items.AddAsync(item);
            await _context.SaveChangesAsync();

            return item.ToDto();
        }

        public async Task DeleteEditionAsync(int id)
        {
            var edition = await _context.Editions.FirstOrDefaultAsync(e => e.Id == id && !e.IsDeleted);
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

        public async Task<IEnumerable<ItemDto>> GetAvailableItemsAsync(int editionId)
        {
            var items = await _context.Items
                            .Where(i => i.EditionId == editionId && i.ItemStatus == ItemStatus.Available)
                            .OrderBy(i => i.Barcode)
                            .AsNoTracking()
                            .ToListAsync();

            return items.Select(i => i.ToDto()).ToList();
        }

        public async Task<EditionDto?> GetEditionByIdAsync(int id)
        {
            var edition = await _context.Editions
                                .Include(e => e.Items.Where(i => !i.IsDeleted))
                                .FirstOrDefaultAsync(e => e.Id == id && !e.IsDeleted);

            if (edition == null)
                return null;

            return edition.ToDto();
        }

        public async Task<IEnumerable<EditionDto>> GetEditionsByBookAsync(int bookId)
        {
            var bookExists = await _context.Books.AnyAsync(b => b.Id == bookId);
            if (!bookExists)
                throw new KeyNotFoundException("Book not found");

            var editions = await _context.Editions
                            .Where(e => e.BookId == bookId && !e.IsDeleted)
                            .OrderBy(e => e.EditionNumber)
                            .AsNoTracking()
                            .ToListAsync();

            return editions.Select(e => e.ToDto()).ToList();
        }

        public async Task<ItemDto?> GetItemByIdAsync(int id)
        {
            var item = await _context.Items.FirstOrDefaultAsync(i => i.Id == id && !i.IsDeleted);
            if (item == null)
                return null;

            return item.ToDto();
        }

        public async Task<IEnumerable<ItemDto>> GetItemsByBookAsync(int bookId)
        {
            var editonExists = await _context.Books.FirstOrDefaultAsync(e => e.Id == bookId);
            if (editonExists == null)
                throw new KeyNotFoundException($"Book with id {bookId} not found");

            var items = await _context.Items
                                .Include(i => i.Edition)
                                .Where(i => i.Edition.BookId == bookId)
                                .OrderBy(i => i.Price)
                                .AsNoTracking()
                                .ToListAsync();

            return items.Select(i => i.ToDto()).ToList();

        }

        public async Task<IEnumerable<ItemDto>> GetItemsByEditionAsync(int editionId)
        {
            var editonExists = await _context.Editions.FirstOrDefaultAsync(e => e.Id == editionId && !e.IsDeleted);
            if (editonExists == null)
                throw new InvalidOperationException($"Edition with this id : {editionId} does not exists");

            var items = await _context.Items
                                .OrderBy(i => i.Price)
                                .AsNoTracking()
                                .Where(i => i.EditionId == editionId)
                                .ToListAsync();

            return items.Select(i => i.ToDto()).ToList();
        }

        public async Task<EditionDto> UpdateEditionAsync(int id, EditionUpdateDto dto)
        {
            var edition = await _context.Editions
                .FirstOrDefaultAsync(e => e.Id == id);

            if (edition == null)
                throw new KeyNotFoundException("Edition not found");

            if (dto.EditionNumber != null)
            {
                var duplicate = await _context.Editions
                    .AnyAsync(e => e.BookId == edition.BookId &&
                                   e.EditionNumber == dto.EditionNumber &&
                                   e.Id != id);

                if (duplicate)
                    throw new InvalidOperationException(
                        $"Edition {dto.EditionNumber} already exists for this book");
            }

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

            await _context.SaveChangesAsync();

            return edition.ToDto();
        }

        public async Task<ItemDto> UpdateItemAsync(int id, ItemUpdateDto dto)
        {
            var item = await _context.Items
                .FirstOrDefaultAsync(i => i.Id == id);

            if (item == null)
                throw new KeyNotFoundException("Item not found");

            if (dto.Barcode != null)
            {
                var barcodeExists = await _context.Items
                    .AnyAsync(i => i.Barcode == dto.Barcode && i.Id != id);

                if (barcodeExists)
                    throw new InvalidOperationException("Barcode already exists");
            }

            if (dto.Barcode != null)
                item.Barcode = dto.Barcode;

            if (dto.Notes != null)
                item.Notes = dto.Notes;

            if (dto.Location != null)
                item.Location = dto.Location;

            if (dto.ItemStatus != null)
                item.ItemStatus = Enum.Parse<ItemStatus>(dto.ItemStatus);

            if (dto.Price != null)
                item.Price = dto.Price.Value;

            await _context.SaveChangesAsync();

            return item.ToDto();
        }
    }
}