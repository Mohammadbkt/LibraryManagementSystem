using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace library.Dtos.Circulation.User
{
    public class ActiveSessionDto
    {
        public int Id { get; set; }
        public string DeviceName { get; set; } = null!;
        public string IpAddress { get; set; } = null!;
        public DateTime ExpiresAt { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}