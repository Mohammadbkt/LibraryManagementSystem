using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace library.Dtos.Circulation.Bookmark
{
    public class AddBookmarkDto
    {
        public int BookId { get; set; }
        public string? Notes { get; set; }
    }
}