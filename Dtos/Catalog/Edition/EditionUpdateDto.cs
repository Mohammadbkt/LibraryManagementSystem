using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using library.Models.Enums;

namespace library.Dtos.Catalog.Edition
{
    public class EditionUpdateDto
    {
        public string? ISBN { get; set; }

        public int? EditionNumber { get; set; }

        public int? PublicationYear { get; set; }

        public string? Language { get; set; }

        public string? CoverImageUrl { get; set; }

        public int? PageCount { get; set; }

    }
}