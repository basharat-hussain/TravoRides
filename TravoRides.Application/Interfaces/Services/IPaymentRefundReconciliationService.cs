namespace TravoRides.Application.Interfaces.Services
{
    public interface IPaymentRefundReconciliationService
    {
        Task ReconcilePendingRefundsAsync(CancellationToken cancellationToken = default);
        Task ReconcileRefundAsync(Guid refundId, CancellationToken cancellationToken = default);
    }
}

