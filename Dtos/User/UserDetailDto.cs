using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using library.Dtos.Circulation.User;

namespace library.Dtos.User
{
    public class UserDetailDto : UserSummaryDto
    {
        public string? PhoneNumber { get; set; }
        public DateTime? LastLoginDate { get; set; }
        public int ActiveLoansCount { get; set; }
        public int TotalFinesAmount { get; set; }

        public List<string>? Roles { get; set; }
    }
}