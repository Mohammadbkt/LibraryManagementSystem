using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using library.Dtos.Catalog.Edition;
using library.Models.Entities;
using library.Models.Enums;

namespace library.Mappers
{
    public static class EditionMappers
    {
        public static EditionDto ToDto(this Edition edition)
        {
            return new EditionDto
            {
                Id = edition.Id,
                BookId = edition.BookId,
                EditionNumber = edition.EditionNumber,
                PublicationYear = edition.PublicationYear,
                ISBN = edition.ISBN,
                CoverImageUrl = edition.CoverImageUrl ?? edition.Book?.CoverImageUrl,
                PageCount = edition.PageCount,
                Language = edition.Language,
                Format = edition.Format,
                TotalItems = edition.Items.Count,
                AvailableItems = edition.Items
                    .Count(i => i.ItemStatus == ItemStatus.Available)
            };
        }

        public static Edition ToEntity(this EditionCreateDto dto)
        {
            return new Edition
            {
                BookId = dto.BookId,
                EditionNumber = dto.EditionNumber,
                PublicationYear = dto.PublicationYear,
                ISBN = dto.ISBN ?? string.Empty,
                CoverImageUrl = dto.CoverImageUrl,
                PageCount = dto.PageCount,
                Language = dto.Language ?? "English",
                Format = dto.Format
            };
        }
    }
}