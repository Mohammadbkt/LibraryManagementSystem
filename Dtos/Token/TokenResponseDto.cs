using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace library.Dtos.Token
{
    public class TokenResponseDto
    {
        
        public string AccessToken { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
        public DateTime ExpiresAt { get; set; }
        public string UserId {get; set;} = string.Empty;
        public string Email {get; set;} = string.Empty;
        public List<String> Roles {get; set;} = [];
    }
}