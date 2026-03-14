using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace library.Dtos.Auth
{
    public class OtpResultDto
    {
        public string OtpCode { get; set; } = string.Empty;
        public DateTime ExpiresAt { get; set; }
        public string? MaskedOtp => $"******{OtpCode[^2..]}";
        public int ExpiresInMinutes { get; set; }
    }
}