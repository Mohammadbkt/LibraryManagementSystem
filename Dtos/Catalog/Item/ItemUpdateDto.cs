using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace library.Dtos.Catalog.Item
{
    public class ItemUpdateDto
    {
        public string? Barcode { get; set; }

        public string? ItemStatus { get; set; }

        public decimal? Price { get; set; }

        public string? Notes { get; set; }

        public string? Location { get; set; }
    }
}