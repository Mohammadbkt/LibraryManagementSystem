using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using library.Dtos.Catalog.Book;
using library.Models.Entities;
using library.Models.Enums;

namespace library.Mappers
{
    public static class BookMappers
    {
        public static BookSummaryDto ToSummaryDto(this Book book)
        {
            return new BookSummaryDto
            {
                Id = book.Id,
                Title = book.Title,
                CoverImageUrl = book.CoverImageUrl,
                Authors = book.BookAuthors
                    .OrderBy(ba => ba.AuthorOrder)
                    .Select(ba => ba.Author.FullName)
                    .ToList(),
                Categories = book.BookCategories
                    .Select(bc => bc.Category.Name)
                    .ToList(),
                PublisherName = book.Publisher?.Name,
                IsAvailable = book.Editions
                    .Any(e => e.Items
                        .Any(i => i.ItemStatus == ItemStatus.Available))
            };
        }

        public static BookDetailDto ToDetailDto(this Book book)
        {
            return new BookDetailDto
            {
                Id = book.Id,
                Title = book.Title,
                Description = book.Description,
                CoverImageUrl = book.CoverImageUrl,
                Authors = book.BookAuthors
                    .OrderBy(ba => ba.AuthorOrder)
                    .Select(ba => ba.Author.ToDto())
                    .ToList(),
                Categories = book.BookCategories
                    .Select(bc => bc.Category.ToDto())
                    .ToList(),
                Publisher = book.Publisher?.ToDto(),
                Editions = book.Editions
                    .Select(e => e.ToDto())
                    .ToList(),
                IsAvailable = book.Editions
                    .Any(e => e.Items
                        .Any(i => i.ItemStatus == ItemStatus.Available))
            };
        }

        public static Book ToEntity(this BookCreateDto dto)
        {
            return new Book
            {
                Title = dto.Title,
                Description = dto.Description,
                CoverImageUrl = dto.CoverImageUrl,
                PublisherId = dto.PublisherId
            };
        }

        public static void ApplyUpdate(this BookUpdateDto dto, Book book)
        {
            if (dto.Title != null)
                book.Title = dto.Title;

            if (dto.Description != null)
                book.Description = dto.Description;

            if (dto.CoverImageUrl != null)
                book.CoverImageUrl = dto.CoverImageUrl;

            if (dto.PublisherId.HasValue)
                book.PublisherId = dto.PublisherId.Value;
        }

    }
}