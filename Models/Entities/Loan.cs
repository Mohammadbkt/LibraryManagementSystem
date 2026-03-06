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
        
        [Required]
        public int ItemId { get; set; }
        
        [Required]
        public string UserId { get; set; } = string.Empty;
        
        public DateTime BorrowDate { get; set; } = DateTime.UtcNow;
        public DateTime DueDate { get; set; }
        public DateTime? ReturnDate { get; set; }
        
        public LoanStatus Status { get; set; } = LoanStatus.Active;
        
        public string? Notes { get; set; }
        
        public Item Item { get; set; } = null!;
        public User User { get; set; } = null!;
        public Fine? Fine { get; set; }
    }
}