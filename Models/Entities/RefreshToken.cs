using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace library.Models.Entities
{
    public class RefreshToken
    {

        public int Id { get; set; }
        public string UserId { get; set; } = string.Empty;
        public string Token { get; set; } = null!;
        public string? DeviceName { get; set; }
        public string? IpAddress { get; set; }
        public DateTime ExpiresAt { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? RevokedAt { get; set; }
        public string? ReplacedByToken { get; set; }
        public bool IsActive => RevokedAt == null && DateTime.UtcNow < ExpiresAt;
        
        public User User { get; set; } = null!;
    }
}
