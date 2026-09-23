using Microsoft.EntityFrameworkCore;
using TravoRides.Application.Common.Exceptions;
using TravoRides.Application.Interfaces.Services;
using TravoRides.Infrastructure.Context;

namespace TravoRides.Infrastructure.Services
{
    public class PaymentNumberGenerator : IPaymentNumberGenerator
    {
        private readonly ApplicationDbContext _dbContext;

        public PaymentNumberGenerator(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<string> GenerateAsync(CancellationToken cancellationToken)
        {
            try
            {
                var connection = _dbContext.Database.GetDbConnection();
                if (connection.State != System.Data.ConnectionState.Open)
                {
                    await _dbContext.Database.OpenConnectionAsync(cancellationToken);
                }

                using var command = connection.CreateCommand();
                command.CommandText = @"
                    IF NOT EXISTS (SELECT 1 FROM sys.sequences WHERE name = 'PaymentNumberSequence')
                    BEGIN
                        CREATE SEQUENCE dbo.PaymentNumberSequence START WITH 1 INCREMENT BY 1;
                    END
                    SELECT NEXT VALUE FOR dbo.PaymentNumberSequence;";

                var nextVal = await command.ExecuteScalarAsync(cancellationToken);
                if (nextVal != null)
                {
                    var number = Convert.ToInt64(nextVal);
                    return $"PAY-{number:D8}";
                }
            }
            catch
            {
                return $"PAY-{DateTime.UtcNow:yyyyMMddHHmmss}-{Random.Shared.Next(1000, 9999)}";
            }

            throw new ConflictException("Failed to generate payment number.");
        }
    }
}

