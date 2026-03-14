using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace library.Models.Entities
{
    public class Otp
    {
        public int Id { get; set; }
        public string UserId { get; set; } = string.Empty;
        public string OtpHash { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime ExpiresAt { get; set; }
        public DateTime? VerifiedAt { get; set; }
        public bool IsUsed { get; set; }
        public int FailedAttempts { get; set; }
        public string? IpAddress { get; set; }
        public string? RequestId { get; set; }
        
        // Navigation property
        public virtual User? User { get; set; }
    }
}