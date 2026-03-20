using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using library.Dtos.Catalog.Author;
using library.Dtos.Catalog.Category;
using library.Dtos.Catalog.Edition;
using library.Dtos.Catalog.Publisher;

namespace library.Dtos.Catalog.Book
{
    public class BookDetailDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? CoverImageUrl { get; set; }
        public IEnumerable<AuthorDto> Authors { get; set; } = [];
        public IEnumerable<CategoryDto> Categories { get; set; } = [];
        public PublisherDto? Publisher { get; set; }
        public IEnumerable<EditionDto> Editions { get; set; } = [];
        public bool IsAvailable { get; set; }
    }
}