using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace library.Dtos.Auth
{
    public class VerifyOtpRequestDto
    {
        public string UserId { get; set; } = string.Empty;

        public string OtpCode { get; set; } = string.Empty;
    }
}