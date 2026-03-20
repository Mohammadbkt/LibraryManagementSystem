using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace library.Dtos.Auth
{
    public class ResendOtpResponseDto
    {
        
    public bool IsSuccess { get; set; }
    public string Message { get; set; } = string.Empty;
    }
}