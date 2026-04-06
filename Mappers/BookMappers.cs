using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using library.Dtos.Catalog.Author;
using library.Dtos.Catalog.Book;
using library.Dtos.Catalog.Category;
using library.Dtos.Catalog.Edition;
using library.Dtos.Catalog.Publisher;
using library.Models.Entities;
using library.Models.Enums;

namespace library.Mappers
{
    public static class BookMappers
    {
        public static Expression<Func<Book, BookSummaryDto>> ToSummaryDto()
        {
            return b => new BookSummaryDto
            {
                Id = b.Id,
                Title = b.Title,
                CoverImageUrl = b.CoverImageUrl,
                Authors = b.BookAuthors
                    .OrderBy(ba => ba.AuthorOrder)
                    .Select(ba => ba.Author.FullName)
                    .ToList(),
                Categories = b.BookCategories
                    .Select(bc => bc.Category.Name)
                    .ToList(),
                PublisherName = b.Publisher != null ? b.Publisher.Name : null,
                IsAvailable = b.Editions
                    .Any(e => e.Items
                        .Any(i => i.ItemStatus == ItemStatus.Available))
            };
        }

        public static Expression<Func<Book, BookDetailDto>> ToDetailDto()
        {
            return b => new BookDetailDto
            {
                Id = b.Id,
                Title = b.Title,
                Description = b.Description,
                CoverImageUrl = b.CoverImageUrl,
                Authors = b.BookAuthors
                .OrderBy(ba => ba.AuthorOrder)
                .Select(ba => new AuthorDto
                {
                    Id = ba.Author.Id,
                    FullName = ba.Author.FullName,
                    Bio = ba.Author.Biography,
                    Nationality = ba.Author.Nationality
                })
                .ToList(),

                Categories = b.BookCategories
                .Select(bc => new CategoryDto
                {
                    Id = bc.Category.Id,
                    Name = bc.Category.Name,
                    Description = bc.Category.Description,
                    ParentId = bc.Category.ParentId,
                    SortOrder = bc.Category.SortOrder
                })
                .ToList(),

                Publisher = b.Publisher == null ? null : new PublisherDto
                {
                    Id = b.Publisher.Id,
                    Name = b.Publisher.Name,
                    Website = b.Publisher.Website
                },

                Editions = b.Editions
            .Select(e => new EditionDto
            {
                Id = e.Id,
                BookId = e.BookId,
                BookTitle = e.Book.Title,
                ISBN = e.ISBN,
                EditionNumber = e.EditionNumber,
                PublicationYear = e.PublicationYear,
                CoverImageUrl = e.CoverImageUrl,
                Language = e.Language,
                PageCount = e.PageCount,
                Format = e.Format,
                TotalItems = e.Items.Count(),
                AvailableItems = e.Items.Count(i => i.ItemStatus == ItemStatus.Available)
            })
            .ToList(),

                IsAvailable = b.Editions
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