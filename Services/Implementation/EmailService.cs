using System;
using System.Threading.Tasks;
using library.Services.Interface;
using library.Models.Configurations;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;
using MailKit.Net.Smtp;
using MailKit.Security;

namespace library.Services.Implementation
{
    public class EmailService : IEmailService
    {
        private readonly EmailConfig _emailConfig;
        private readonly ILogger<EmailService> _logger;

        public EmailService(IOptions<EmailConfig> emailConfig, ILogger<EmailService> logger)
        {
            _emailConfig = emailConfig.Value;
            _logger = logger;
        }

        public async Task SendEmailAsync(string recipient, string subject, string body)
        {
            if (string.IsNullOrWhiteSpace(recipient))
                throw new ArgumentException("Recipient email cannot be empty", nameof(recipient));
            
            if (string.IsNullOrWhiteSpace(subject))
                throw new ArgumentException("Email subject cannot be empty", nameof(subject));

            try
            {
                ValidateConfiguration();

                var message = CreateEmailMessage(recipient, subject, body);
                
                using var client = new SmtpClient();
                
                await ConnectAndAuthenticateAsync(client);
                await client.SendAsync(message);
                await client.DisconnectAsync(true);

                _logger.LogInformation("Email sent successfully to {Recipient}", recipient);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send email to {Recipient}", recipient);
                throw; // Rethrow to let caller handle it
            }
        }

        private void ValidateConfiguration()
        {
            if (string.IsNullOrEmpty(_emailConfig.FromAddress))
                throw new InvalidOperationException("Email configuration: FromAddress is missing");
            
            if (string.IsNullOrEmpty(_emailConfig.SmtpServer))
                throw new InvalidOperationException("Email configuration: SmtpServer is missing");
            
            if (_emailConfig.Port <= 0)
                throw new InvalidOperationException("Email configuration: Port is invalid");
            
            if (string.IsNullOrEmpty(_emailConfig.Username))
                throw new InvalidOperationException("Email configuration: Username is missing");
            
            if (string.IsNullOrEmpty(_emailConfig.Password))
                throw new InvalidOperationException("Email configuration: Password is missing");
        }

        private MimeMessage CreateEmailMessage(string recipient, string subject, string body)
        {
            var message = new MimeMessage();
            
            message.From.Add(new MailboxAddress(
                _emailConfig.FromName ?? "Default Name", 
                _emailConfig.FromAddress
            ));
            
            message.To.Add(new MailboxAddress("", recipient));
            message.Subject = subject;
            message.Body = new TextPart("plain") { Text = body };

            return message;
        }

        private async Task ConnectAndAuthenticateAsync(SmtpClient client)
        {
            await client.ConnectAsync(
                _emailConfig.SmtpServer, 
                _emailConfig.Port, 
                SecureSocketOptions.StartTls
            );
            
            await client.AuthenticateAsync(_emailConfig.Username, _emailConfig.Password);
        }
    }
}