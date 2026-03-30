using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using library.Models.Enums;

namespace library.Dtos.Circulation.Fine
{
    public class FineDto
    {
        public int Id { get; set; }
        public int LoanId { get; set; }
        public string BookTitle { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public DateTime IssuedDate { get; set; }
        public DateTime? PaidDate { get; set; }
        public FineStatus Status { get; set; }
        public string? Reason { get; set; }
    }
}