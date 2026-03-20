using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using library.Dtos.Catalog.Book;

namespace library.Dtos.Catalog.Author
{
    public class AuthorDetailDto
    {
        public int Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string? Bio { get; set; }
        public string? Nationality { get; set; }
        public IEnumerable<BookSummaryDto> Books { get; set; } = [];
    }
}