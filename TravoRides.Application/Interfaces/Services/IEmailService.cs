namespace TravoRides.Application.Interfaces.Services
{
    public interface IEmailService
    {
        Task SendEmailAsync(string to, string subject, string body, bool withHeaderLogo = true, CancellationToken cancellationToken = default);
    }
}
