using System.Net;
using TravoRides.Application.Interfaces.Services;

namespace TravoRides.Infrastructure.Services
{
    public class EmailTemplateService : IEmailTemplateService
    {
        private const string TemplatesFolder = "Templates";

        public async Task<string> GetEmailOTPVerificationTemplateAsync(string otp, int expiryMinutes)
        {
            var path = Path.Combine(AppContext.BaseDirectory, "Templates", "Email", "email-otp-verification-template.html");

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

            html = html.Replace("{{NAME}}", WebUtility.HtmlEncode(name))
                       .Replace("{{SUBJECT}}",WebUtility.HtmlEncode(subject))
                       .Replace("{{MESSAGE}}",WebUtility.HtmlEncode(message))
                       .Replace("{{PHONE}}", WebUtility.HtmlEncode(phone));

            return html;
        }

        public async Task<string> GetBookingConfirmationTemplateAsync(string name, string bookingId, DateTime bookingDate, string cabName, string cabType, decimal totalAmount)
        {
            var path = Path.Combine(GetFullTemplatePath("email"), "BookingConfirmationTemplate.html");
            var html = await File.ReadAllTextAsync(path);
            html = html.Replace("{{NAME}}",WebUtility.HtmlEncode(name))
                       .Replace("{{BOOKING_ID}}", WebUtility.HtmlEncode(bookingId))
                       .Replace("{{BOOKING_DATE}}", bookingDate.ToString("f"))
                       .Replace("{{CAB_NAME}}", WebUtility.HtmlEncode(cabName))
                       .Replace("{{CAB_TYPE}}", WebUtility.HtmlEncode(cabType))
                       .Replace("{{TOTAL_AMOUNT}}", totalAmount.ToString("C"));
            return html;
        }

        public async Task<string> GetBookingCancellationTemplateAsync(string name, string bookingId, DateTime bookingDate, string cabName, string cabType, decimal totalAmount)
        {
            var path = Path.Combine(GetFullTemplatePath("email"), "booking-cancellation-template.html");
            var html = await File.ReadAllTextAsync(path);
            html = html.Replace("{{NAME}}", WebUtility.HtmlEncode(name))
                       .Replace("{{BOOKING_ID}}", WebUtility.HtmlEncode(bookingId))
                       .Replace("{{BOOKING_DATE}}", bookingDate.ToString("f"))
                       .Replace("{{CAB_NAME}}", WebUtility.HtmlEncode(cabName))
                       .Replace("{{CAB_TYPE}}", WebUtility.HtmlEncode(cabType))
                       .Replace("{{TOTAL_AMOUNT}}", totalAmount.ToString("C"));
            return html;
        }
        public async Task<string> GetBookingCompletionTemplateAsync(string name, string bookingId, DateTime bookingDate, string cabName, string cabType, decimal totalAmount)
        {
            var path = Path.Combine(GetFullTemplatePath("email"), "booking-completion-template.html");
            var html = await File.ReadAllTextAsync(path);
            html = html.Replace("{{NAME}}", WebUtility.HtmlEncode(name))
                       .Replace("{{BOOKING_ID}}", WebUtility.HtmlEncode(bookingId))
                       .Replace("{{BOOKING_DATE}}", bookingDate.ToString("f"))
                       .Replace("{{CAB_NAME}}", WebUtility.HtmlEncode(cabName))
                       .Replace("{{CAB_TYPE}}", WebUtility.HtmlEncode(cabType))
                       .Replace("{{TOTAL_AMOUNT}}", totalAmount.ToString("C"));
            return html;
        }
        
    }
}