using System.Linq.Expressions;
using library.Dtos.Catalog.Author;
using library.Dtos.Catalog.Book;
using library.Models.Entities;

namespace library.Mappers
{
    public static class AuthorMapper
    {
        public static Expression<Func<Author, AuthorDto>> ToDto()
        {
            return a => new AuthorDto
            {
                Id = a.Id,
                FullName = a.FullName,
                Bio = a.Biography,
                Nationality = a.Nationality
            };
        }

        public static Expression<Func<Author, AuthorDetailDto>> ToDetailDto()
        {
            return a => new AuthorDetailDto
            {
                Id = a.Id,
                FullName = a.FullName,
                Bio = a.Biography,
                Nationality = a.Nationality,
                Books = a.BookAuthors.Select(ba => new BookSummaryDto
                {
                    Id = ba.Book.Id,
                    Title = ba.Book.Title,
                    CoverImageUrl = ba.Book.CoverImageUrl,
                    PublisherName = ba.Book.Publisher != null ? ba.Book.Publisher.Name : null,
                    Authors = ba.Book.BookAuthors
                        .OrderBy(ba => ba.AuthorOrder)
                        .Select(ba => ba.Author.FullName)
                        .ToList(),

                    Categories = ba.Book.BookCategories
                        .Select(bc => bc.Category.Name),

                    IsAvailable = ba.Book.Editions
                        .Any(e => e.Items
                            .Any(i => i.ItemStatus == Models.Enums.ItemStatus.Available))
                }).ToList()
            };
        }

        public static Author ToEntity(this AuthorCreateDto dto)
        {
            return new Author
            {
                FullName = dto.FullName,
                Biography = dto.Bio,
                Nationality = dto.Nationality,
            };
        }


    }
}