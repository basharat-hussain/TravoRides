using Microsoft.EntityFrameworkCore;
using TravoRides.Application.Repositories;
using TravoRides.Domain.Entities;
using TravoRides.Infrastructure.Context;

namespace TravoRides.Infrastructure.Repository
{
    public class PaymentWebhookRepository : GenericRepository<PaymentWebhook>, IPaymentWebhookRepository
    {
        private readonly ApplicationDbContext context;

        public PaymentWebhookRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
            this.context = dbContext;
        }

        public async Task<PaymentWebhook?> GetByEventIdAsync(string eventId, CancellationToken cancellationToken = default)
        {
            return await context.PaymentWebhooks
                .FirstOrDefaultAsync(x => x.EventId == eventId, cancellationToken);
        }

        public async Task<Payment?> GetByGatewayOrderIdAsync(string gatewayOrderId, CancellationToken cancellationToken = default)
        {
            return await context.Payments
                .FirstOrDefaultAsync(x => x.GatewayOrderId == gatewayOrderId, cancellationToken);
        }

        public async Task<PaymentWebhook?> GetForProcessingAsync(string eventId, CancellationToken cancellationToken = default)
        {
            return await context.PaymentWebhooks
                .FromSqlInterpolated($""" SELECT * FROM PaymentWebhooks WITH (UPDLOCK, ROWLOCK) WHERE EventId = {eventId}""")
                .FirstOrDefaultAsync(cancellationToken);
        }
    }
}

