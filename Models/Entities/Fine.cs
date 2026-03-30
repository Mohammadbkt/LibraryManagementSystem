using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using library.Models.Enums;

namespace library.Models.Entities
{
    public class Fine
    {
        public int Id { get; set; }

        public int LoanId { get; set; }

        public string UserId { get; set; } = string.Empty;

        public decimal Amount { get; set; }
        public DateTime IssuedDate { get; set; } = DateTime.UtcNow;
        public DateTime? PaidDate { get; set; }

        public FineStatus Status { get; set; } = FineStatus.Unpaid;
        public string? Reason { get; set; }
        public bool IsDeleted { get; set; } = false;
        public DateTime? DeletedAt { get; set; }

        public Loan Loan { get; set; } = null!;
        public User User { get; set; } = null!;
    }
}