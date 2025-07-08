using GIL_Agent_Portal.Models;
using GIL_Agent_Portal.Services.Intetrface;
using Microsoft.Extensions.Options;
using MimeKit;
using System.Net.Mail;

namespace GIL_Agent_Portal.Services
{
    public class EmailService : IEmailService
    {
        private readonly SmtpSettings _smtpSettings;
        private IConfiguration _configuration;

        public EmailService(IOptions<SmtpSettings> smtpSettings, IConfiguration configuration)
        {
            _smtpSettings = smtpSettings.Value;
            _configuration = configuration;
        }
        public void SendEmail(string toEmail, string subject, string message)
        {
            var email = new MimeMessage();
            email.From.Add(new MailboxAddress(_smtpSettings.SenderName, _smtpSettings.SenderEmail));
            email.To.Add(new MailboxAddress("", toEmail));
            email.Subject = subject;
            email.Body = new TextPart("plain") { Text = message };

            using var smtp = new MailKit.Net.Smtp.SmtpClient();
            smtp.Connect(_smtpSettings.Server, _smtpSettings.Port, MailKit.Security.SecureSocketOptions.StartTls);
            smtp.Authenticate(_smtpSettings.Username, _smtpSettings.Password);
            smtp.Send(email);
            smtp.Disconnect(true);
        }
    }
}
