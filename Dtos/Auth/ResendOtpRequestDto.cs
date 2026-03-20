using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace library.Dtos.Auth
{
    public class ResendOtpRequestDto
    {
        public string UserId { get; set; } = string.Empty;
    }
}