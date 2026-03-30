using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using library.Models.Enums;

namespace library.Dtos.Circulation.Loan
{
    public class CheckoutDto
    {
        public int ItemId { get; set; }

        public int LoanDurationDays { get; set; } = 14;
    }
}