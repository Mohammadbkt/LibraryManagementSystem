using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;

namespace library.Dtos.Catalog.Category
{
    public class CategoryDto
    {
        public int Id {get; set;}
        public string Name {get; set;} = string.Empty;
        public string? Description { get; set; }

    }
}