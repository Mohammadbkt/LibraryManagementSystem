using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace library.Dtos.Catalog.Book
{
    public class BookSummaryDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? CoverImageUrl { get; set; }
        public IEnumerable<string> Authors { get; set; } = [];
        public IEnumerable<string> Categories { get; set; } = [];
        public string? PublisherName { get; set; }
        public bool IsAvailable { get; set; }
    }
}