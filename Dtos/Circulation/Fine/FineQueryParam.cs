using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace library.Dtos.Circulation.Fine
{
    public class FineQueryParam
    {
        public string? Status { get; set; }
        public decimal Amount { get; set; }

        private int _page = 1;
        public int Page
        {
            get => _page;
            set => _page = value < 1 ? 1 : value;
        }

        private int _pageSize = 10;
        public int PageSize
        {
            get => _pageSize;
            set => _pageSize = value > 50 ? 50 : value < 1 ? 1 : value;
        }
    }
}