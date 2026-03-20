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
        public string BookTitle { get; set; } = string.Empty;
        public string? Barcode { get; set; }
        public string Condition { get; set; } = string.Empty;
        public bool IsAvailable { get; set; }
        public DateTime AcquiredAt { get; set; }
    }
}