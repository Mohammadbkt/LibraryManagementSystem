using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using library.Models.Enums;

namespace library.Dtos.Circulation.Loan
{
    public class UpdateLoanDto
    {
        public int ItemId { get; set; }
        public string UserId { get; set; } = string.Empty;
        public DateTime DueDate { get; set; }
        public DateTime? ReturnDate { get; set; }
        public LoanStatus Status { get; set; } = LoanStatus.Active;
        public string? Notes { get; set; }

    }
}