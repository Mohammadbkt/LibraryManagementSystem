using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using library.Models.Enums;

namespace library.Dtos.Catalog.Edition
{
    public class EditionDto
    {
        public int Id { get; set; }
        public int BookId { get; set; }
        public string BookTitle { get; set; } = string.Empty;
        public string? ISBN { get; set; }
        public int EditionNumber { get; set; }
        public int? PublicationYear { get; set; }
        public string? CoverImageUrl {get; set;}
        public string? Language { get; set; }
        public int? PageCount {get; set;}
        public EditionFormat Format {get; set;}
        public int TotalItems { get; set; }
        public int AvailableItems { get; set; }
    }
}