using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace library.Dtos.Circulation.Review
{
    public class ReviewDto
    {
        public int Id { get; set; }
        public int BookId { get; set; }
        public int LoanId {get; set;}
        public string BookTitle { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;
        public string UserFullName { get; set; } = string.Empty;
        public int Rating { get; set; }
        public string? Comment { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}