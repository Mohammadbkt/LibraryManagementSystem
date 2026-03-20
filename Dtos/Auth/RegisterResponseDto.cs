using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace library.Dtos.Auth
{
    public class RegisterResponseDto
    {
        public bool IsSuccess { get; set; }

        public string? UserId { get; set; }
        public string? Email { get; set; }

        public string? Message { get; set; }
    }
}