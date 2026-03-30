using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using library.Models.Enums;

namespace library.Models.Entities
{
    public class Loan
    {
        public int Id { get; set; }

        public int ItemId { get; set; }

        public string UserId { get; set; } = string.Empty;

        public DateTime BorrowDate { get; set; } = DateTime.UtcNow;
        public DateTime DueDate { get; set; }
        public DateTime? ReturnDate { get; set; }

        public LoanStatus Status { get; set; } = LoanStatus.Active;
        public bool IsDeleted { get; set; } = false;
        public DateTime? DeletedAt { get; set; }
        public DateTime? UpdatedAt {get; set;}
        public string? Notes { get; set; }

        public Item Item { get; set; } = null!;
        public User User { get; set; } = null!;
        public Fine? Fine { get; set; }
        public Edition Edition {get; set;} = null!;
        public Review? Review { get; set; }
    }
}