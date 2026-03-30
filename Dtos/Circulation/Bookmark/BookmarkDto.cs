using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace library.Dtos.Circulation.Bookmark
{
    public class BookmarkDto
    {
        public int BookId { get; set; }
        public string BookTitle { get; set; } = string.Empty;
        public string? CoverImageUrl { get; set; }
        public string? Notes { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}