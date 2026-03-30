using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace library.Dtos.Circulation.Review
{
    public class ReviewQueryParam
    {

        public string? Search { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}