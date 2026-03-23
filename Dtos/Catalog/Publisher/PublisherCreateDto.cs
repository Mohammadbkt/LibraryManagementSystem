using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace library.Dtos.Catalog.Publisher
{
    public class PublisherCreateDto
    {
        public string Name { get; set; } = string.Empty;
        public string? Website { get; set; }
    }
}