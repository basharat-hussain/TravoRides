using TravoRides.Application.Interfaces.Services;

namespace TravoRiders.Infrastructure.Services
{
    public class EmailTemplateService : IEmailTemplateService
    {
        private const string TemplatesFolder = "Templates";

        public async Task<string> GetEmailOTPVerificationTemplateAsync(string otp, int expiryMinutes)
        {
            var path = Path.Combine(AppContext.BaseDirectory, "Templates", "Email", "EmailOtpVerification.html");

            var html = await File.ReadAllTextAsync(path);

            return html.Replace("{{OTP}}", otp).Replace("{{EXPIRY_MINUTES}}", expiryMinutes.ToString());
        }

        public async Task<string> GetForgotPasswordOTPTemplateAsync(string otp, int expiryMinutes)
        {
            var path = Path.Combine(GetFullTemplatePath("email"), "forgot-password-otp-template.html");
            var html = await File.ReadAllTextAsync(path);
            return html.Replace("{{OTP}}", otp).Replace("{{EXPIRY_MINUTES}}", expiryMinutes.ToString());
        }

        private string GetFullTemplatePath(string action)
        {
            var basePath = AppContext.BaseDirectory;
            return action switch
            {
                "email" => Path.Combine(basePath, TemplatesFolder, "Email"),
                "sms" => Path.Combine(basePath, TemplatesFolder, "SMS"),
                "whatsapp" => Path.Combine(basePath, TemplatesFolder, "WhatsApp"),
                _ => throw new ArgumentException($"Unknown action: {action}")
            };
        }

        public async Task<string> GetEnquiryConfirmationTemplateAsync(string name, string subject, string message, string phone)
        {
            var path = Path.Combine(GetFullTemplatePath("email"), "enquiry-confirmation-template.html");

            var html = await File.ReadAllTextAsync(path);

            html = html.Replace("{{NAME}}", System.Net.WebUtility.HtmlEncode(name))
                       .Replace("{{SUBJECT}}", System.Net.WebUtility.HtmlEncode(subject))
                       .Replace("{{MESSAGE}}", System.Net.WebUtility.HtmlEncode(message))
                       .Replace("{{PHONE}}", System.Net.WebUtility.HtmlEncode(phone));

            return html;
        }
    }
}
