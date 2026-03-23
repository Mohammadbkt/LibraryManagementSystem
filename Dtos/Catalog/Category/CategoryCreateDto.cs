using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace library.Dtos.Catalog.Category
{
    public class CategoryCreateDto
    {
        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }
        public int? ParentId { get; set; }
        public int SortOrder { get; set; } = 0;
    }
}