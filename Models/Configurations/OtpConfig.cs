using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace library.Models.Configurations
{
    public class OtpConfig
    {
        public int OtpLength { get; set; } = 6;
        public int ExpiryMinutes { get; set; } = 15;
        public int MaxFailedAttempts { get; set; } = 3;
        public bool RequireIpValidation { get; set; } = false;
    }
}