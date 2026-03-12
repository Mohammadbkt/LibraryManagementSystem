using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace library.Dtos.Auth
{
    public class RegisterDto
    {
        public string Email {get; set;} = string.Empty;
        public string PasswordHash {get; set;}  = string.Empty;
        public string UserName {get; set;}  = string.Empty;
        public string FirstName {get; set;}  = string.Empty;
        public string LastName {get; set;}  = string.Empty;
        public string ConfirmPassword { get; set; } = string.Empty;
        public string? PhoneNumber {get;set;}

    }
}