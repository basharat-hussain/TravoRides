namespace TravoRides.Application.Interfaces.Services
{
    public interface IPaymentNumberGenerator
    {
        Task<string> GenerateAsync(CancellationToken cancellationToken);
    }
}

