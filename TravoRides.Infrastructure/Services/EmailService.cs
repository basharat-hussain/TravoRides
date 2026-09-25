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

        public async Task SendEmailAsync(string to, string subject, string body, bool withHeaderLogo = true, string? cc = null, CancellationToken cancellationToken = default)
        {
            var email = new MimeMessage
            {
                From = { new MailboxAddress(_settings.FromName, _settings.FromEmail) },
                To = { MailboxAddress.Parse(to) },
                Subject = subject,
            };

            if (!string.IsNullOrWhiteSpace(cc) && !string.Equals(to, cc.Trim(), StringComparison.OrdinalIgnoreCase))
            {
                email.Cc.Add(MailboxAddress.Parse(cc.Trim()));
            }

            var bodyBuilder = new BodyBuilder { HtmlBody = body };

            if (withHeaderLogo)
            {
                var logoPath = Path.Combine(AppContext.BaseDirectory, "Templates", "Email", "Images", "headerlogo.png");
                if (!File.Exists(logoPath))
                {
                    var fallbackPath = Path.Combine(Directory.GetCurrentDirectory(), "Templates", "Email", "Images", "headerlogo.png");
                    if (File.Exists(fallbackPath))
                    {
                        logoPath = fallbackPath;
                    }
                }

                if (File.Exists(logoPath))
                {
                    var logo = bodyBuilder.LinkedResources.Add(logoPath);
                    logo.ContentId = "logoImage";
                    logo.ContentDisposition = new ContentDisposition(ContentDisposition.Inline);

                    if (!body.Contains("cid:logoImage", StringComparison.OrdinalIgnoreCase))
                    {
                        const string headerLogoHtml = "<div style=\"text-align:center; padding:15px; background-color:#ffffff; margin-bottom:15px;\"><img src=\"cid:logoImage\" alt=\"TravoRides\" width=\"140\" style=\"display:inline-block; border:0;\" /></div>";
                        var bodyIndex = body.IndexOf("<body", StringComparison.OrdinalIgnoreCase);
                        if (bodyIndex >= 0)
                        {
                            var closeTagIndex = body.IndexOf('>', bodyIndex);
                            if (closeTagIndex >= 0)
                            {
                                body = body.Insert(closeTagIndex + 1, headerLogoHtml);
                            }
                            else
                            {
                                body = headerLogoHtml + body;
                            }
                        }
                        else
                        {
                            body = headerLogoHtml + body;
                        }

                        bodyBuilder.HtmlBody = body;
                    }
                }
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
