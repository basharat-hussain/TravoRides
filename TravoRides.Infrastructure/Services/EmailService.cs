using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using TravoRides.Application.Interfaces.Services;
using TravoRides.Application.Common.Models;

namespace TravoRides.Infrastructure.Services
{
    public class EmailService : IEmailService
    {
        private readonly EmailSettings _settings;

        public EmailService(IOptions<EmailSettings> settings)
        {
            _settings = settings.Value;
        }

        public async Task SendEmailAsync(string to, string subject, string body, bool withHeaderLogo = true, CancellationToken cancellationToken = default)
        {
            var email = new MimeMessage
            {
                From = { new MailboxAddress(_settings.FromName, _settings.FromEmail) },
                To = { MailboxAddress.Parse(to) },
                Subject = subject,
                // Body = new TextPart("html") { Text = body },

            };

            var bodyBuilder = new BodyBuilder { HtmlBody = body };

            if (withHeaderLogo)
            {
                // Path to image
                var logoPath = Path.Combine(AppContext.BaseDirectory, "Templates", "Email", "Images", "headerlogo.png");
                // Add image as inline resource
                var logo = bodyBuilder.LinkedResources.Add(logoPath);
                logo.ContentId = "logoImage";

            }

            email.Body = bodyBuilder.ToMessageBody();


            using var smtp = new SmtpClient();

            await smtp.ConnectAsync(_settings.Host, _settings.Port, _settings.EnableSsl ? SecureSocketOptions.StartTls : SecureSocketOptions.Auto, cancellationToken);

            await smtp.AuthenticateAsync(_settings.Username, _settings.Password, cancellationToken);

            await smtp.SendAsync(email, cancellationToken);

            await smtp.DisconnectAsync(true, cancellationToken);
        }
    }
}
