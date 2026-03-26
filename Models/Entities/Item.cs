using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using library.Models.Enums;

namespace library.Models.Entities
{
    public class Item
    {
        public int Id { get; set; }

        public DateTime AcquisitionDate { get; set; }
        public ItemStatus ItemStatus { get; set; } = ItemStatus.Available;

        public decimal Price { get; set; }

        public string? Notes { get; set; }

        public string Barcode { get; set; } = string.Empty;

        public string? Location { get; set; }
        public int EditionId { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public bool IsDeleted { get; set; } = false;
        public DateTime? DeletedAt { get; set; }

        public ICollection<Loan> Loans { get; set; } = new List<Loan>();
        public ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();
        public Edition Edition { get; set; } = null!;
    }
}