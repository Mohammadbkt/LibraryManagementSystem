// Services/Interface/IEmailService.cs
using System.ComponentModel.DataAnnotations;
using library.Models;

namespace library.Services.Interface
{
    public interface IEmailService
    {
                Task SendEmailAsync([EmailAddress] string Recipient, string Subject, string Body );

    }
}