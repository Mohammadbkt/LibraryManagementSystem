using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace library.Dtos.Circulation.Review
{
    public class AddReviewDto
    {
        public int BookId { get; set; }

        public int LoanId { get; set; }

        public int Rating { get; set; }

        public string? Comment { get; set; }
    }
}