using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace library.Dtos.Catalog.Item
{
    public class ItemDto
    {
        public int Id { get; set; }
        public int EditionId { get; set; }
        public string Barcode { get; set; } = string.Empty;
        public string ItemStatus { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string? Notes { get; set; }
        public string? Location { get; set; }
        public DateTime AcquisitionDate { get; set; }
    }
}