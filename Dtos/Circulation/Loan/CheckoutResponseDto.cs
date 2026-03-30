using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace library.Dtos.Circulation.Loan
{
    public class CheckoutResponseDto
    {
        public LoanDto Loan { get; set; } = null!;
        public int RemainingLoans { get; set; }
    }
}
