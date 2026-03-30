using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using library.Dtos.Circulation.Bookmark;
using library.Models.Entities;

namespace library.Mappers
{
    public static class BookmarkMappers
    {
        public static Bookmark ToEntity(this AddBookmarkDto dto, string userId)
        {
            return new Bookmark()
            {
                UserId = userId,
                BookId = dto.BookId,
                Notes = dto.Notes
            };
        }

        public static BookmarkDto ToDto(this Bookmark bookmark)
        {
            return new BookmarkDto()
            {
                BookId = bookmark.BookId,
                BookTitle = bookmark.Book.Title,
                CoverImageUrl = bookmark.Book.CoverImageUrl,
                Notes = bookmark.Notes,
                CreatedAt = bookmark.CreatedAt
            };
        }
    }
}