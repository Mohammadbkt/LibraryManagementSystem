using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace library.Dtos.Catalog.Author
{
    public class AuthorCreateDto
    {
        public string FullName { get; set; } = string.Empty;

        public string? Bio { get; set; }

        public string? Nationality { get; set; }
    }
}