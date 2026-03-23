using library.Dtos.Catalog.Author;
using library.Models.Entities;

namespace library.Mappers
{
    public static class AuthorMapper
    {
        public static AuthorDto ToDto(this Author author)
        {
            return new AuthorDto
            {
                Id = author.Id,
                FullName = author.FullName,
                Bio = author.Biography,
                Nationality = author.Nationality
            };
        }

        public static AuthorDetailDto ToDetailDto(this Author author)
        {
            return new AuthorDetailDto
            {
                Id = author.Id,
                FullName = author.FullName,
                Bio = author.Biography,
                Nationality = author.Nationality,
                Books = author.BookAuthors.Select(ba => ba.Book.ToSummaryDto())
            .ToList()
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