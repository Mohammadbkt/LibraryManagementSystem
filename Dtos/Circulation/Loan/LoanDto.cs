using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using library.Models.Enums;

namespace library.Dtos.Circulation.Loan
{
    public class LoanDto
    {
        public int Id { get; set; }
        public string UserId { get; set; } = string.Empty;
        public string UserFullName { get; set; } = string.Empty;
        public int ItemId { get; set; }
        public string Barcode { get; set; } = string.Empty;
        public string BookTitle { get; set; } = string.Empty;
        public DateTime BorrowDate { get; set; }
        public DateTime DueDate { get; set; }
        public DateTime? ReturnDate { get; set; }
        public LoanStatus Status { get; set; }
        public bool HasFine { get; set; }
        public decimal? FineAmount { get; set; }

    }
}