using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace library.Dtos.Circulation.User
{
    public class ToProfileDto
    {
        public string Id { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string Email { get; set; } = null!;
        public DateTime MemberSince { get; set; }
        public DateTime? LastLoginDate { get; set; }    
        public bool IsActive { get; set; }
        public IEnumerable<string> Role { get; set; } = null!;
    }
}