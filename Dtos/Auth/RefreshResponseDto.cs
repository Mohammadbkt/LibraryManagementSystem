using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace library.Dtos.Auth
{
    public class RefreshResponseDto
    {
        public bool IsSuccess { get; set; }

        public string? AccessToken { get; set; }
        public string? RefreshToken { get; set; }

        public DateTime ExpiresAt { get; set; }

        public string? Message { get; set; }
    }
}