using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using library.Models.Enums;

namespace library.Dtos.Catalog.Edition
{
    public class EditionCreateDto
    {
        public int BookId { get; set; }

        public string ISBN { get; set; } = string.Empty;

        public int EditionNumber { get; set; }
        public string? CoverImageUrl { get; set; }

        public int? PageCount { get; set; }

        public int? PublicationYear { get; set; }

        public string? Language { get; set; }
        public EditionFormat Format { get; set; } = EditionFormat.Paperback;

        public int TotalItems { get; set; }
        public int AvailableItems { get; set; } 
    }
}