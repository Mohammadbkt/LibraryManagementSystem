using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace library.Dtos.Catalog.Book
{
    public class BookCreateDto
    {
        public string Title { get; set; } = string.Empty;

        public string? Description { get; set; }

        public string? CoverImageUrl { get; set; }

        public int PublisherId { get; set; }

        public IEnumerable<int> AuthorIds { get; set; } = [];

        public IEnumerable<int> CategoryIds { get; set; } = [];
    }
}