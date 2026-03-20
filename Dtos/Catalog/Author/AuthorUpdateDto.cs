using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace library.Dtos.Catalog.Author
{
    public class AuthorUpdateDto
    {
        public string? FullName { get; set; }

        public string? Bio { get; set; }

        public string? Nationality { get; set; }
    }
}