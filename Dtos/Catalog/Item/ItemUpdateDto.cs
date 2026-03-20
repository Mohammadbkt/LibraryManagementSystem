using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace library.Dtos.Catalog.Item
{
    public class ItemUpdateDto
    {
        public string? Barcode { get; set; }

        public string? Condition { get; set; }

        public bool? IsAvailable { get; set; }
    }
}