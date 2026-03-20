using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace library.Dtos.Catalog.Item
{
    public class ItemCreateDto
    {
        public int EditionId { get; set; }

        public string? Barcode { get; set; }

        public string Condition { get; set; } = string.Empty;

        public DateTime AcquiredAt { get; set; }
    }
}