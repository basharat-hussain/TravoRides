using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using TravoRides.Application.Common.Options;
using TravoRides.Application.DTOs.Payment;
using TravoRides.Application.Interfaces.Payment;
using TravoRides.Application.Interfaces.Services;
using TravoRides.Application.Repositories;
using TravoRides.Domain.Entities;
using TravoRides.Domain.Enums;

namespace TravoRides.Infrastructure.Services
{
    public class PaymentRefundReconcilationService : IPaymentRefundReconciliationService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPaymentGateway _paymentGateway;
        private readonly ILogger<PaymentRefundReconcilationService> _logger;
        private readonly RefundReconciliationOptions _options;

        public PaymentRefundReconcilationService(
            IUnitOfWork unitOfWork,
            IPaymentGateway paymentGateway,
            ILogger<PaymentRefundReconcilationService> logger,
            IOptions<RefundReconciliationOptions> options)
        {
            _unitOfWork = unitOfWork;
            _paymentGateway = paymentGateway;
            _logger = logger;
            _options = options.Value;
        }

        public async Task ReconcilePendingRefundsAsync(CancellationToken cancellationToken = default)
        {
            var refunds = await _unitOfWork.PaymentRefunds.GetPendingRefundsAsync(_options.BatchSize, cancellationToken);

            foreach (var refund in refunds)
            {
                cancellationToken.ThrowIfCancellationRequested();

                try
                {
                    await ReconcileRefundAsync(refund.Id, cancellationToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error reconciling refund {RefundId}", refund.Id);
                }
            }
        }

        public async Task ReconcileRefundAsync(Guid refundId, CancellationToken cancellationToken = default)
        {
            var refund = await _unitOfWork.PaymentRefunds.GetByIdAsync(refundId, cancellationToken);

            if (refund == null)
            {
                _logger.LogWarning("Refund {RefundId} not found.", refundId);
                return;
            }

            _logger.LogInformation("Starting refund reconciliation for {RefundId}. PaymentId={PaymentId}, Amount={Amount}", refund.Id, refund.PaymentId, refund.Amount);

            if (refund.Status != PaymentRefundStatus.Pending)
            {
                _logger.LogInformation("Refund {RefundId} is not pending. Current status: {Status}.", refund.Id, refund.Status);
                return;
            }

            if (refund.RetryCount >= _options.MaxRetries)
            {
                _logger.LogWarning("Refund {RefundId} reached maximum reconciliation retries.", refund.Id);
                return;
            }

            var payment = await _unitOfWork.Payments.GetByIdAsync(refund.PaymentId, cancellationToken);

            if (payment == null)
            {
                _logger.LogError("Payment {PaymentId} not found for refund {RefundId}.", refund.PaymentId, refund.Id);
                return;
            }

            if (string.IsNullOrWhiteSpace(payment.GatewayTransactionId))
            {
                _logger.LogError("Gateway transaction ID missing for payment {PaymentId}.", payment.Id);
                return;
            }

            var gatewayRefunds = await _paymentGateway.GetRefundsAsync(payment.GatewayTransactionId, cancellationToken);
            var gatewayRefund = !string.IsNullOrWhiteSpace(refund.GatewayRefundId)
                ? gatewayRefunds.FirstOrDefault(x => x.GatewayRefundId == refund.GatewayRefundId)
                : null;

            gatewayRefund ??= gatewayRefunds.FirstOrDefault(x => x.Amount.HasValue && x.Amount.Value == refund.Amount && string.Equals(x.Currency, refund.Currency, StringComparison.OrdinalIgnoreCase));

            if (gatewayRefund == null)
            {
                refund.RetryCount++;
                refund.LastCheckedAt = DateTime.UtcNow;
                _unitOfWork.PaymentRefunds.Update(refund);
                await _unitOfWork.SaveChangesAsync(cancellationToken);
                _logger.LogInformation("Refund {RefundId} not found in gateway. Retry count: {RetryCount}.", refund.Id, refund.RetryCount);
                return;
            }

            await MarkRefundProcessedAsync(refund, payment, gatewayRefund, cancellationToken);
            _logger.LogInformation("Refund {RefundId} reconciled successfully. GatewayRefundId={GatewayRefundId}", refund.Id, gatewayRefund.GatewayRefundId);
        }

        private async Task MarkRefundProcessedAsync(PaymentRefund refund, Payment payment, PaymentGatewayRefundResult gatewayRefund, CancellationToken cancellationToken)
        {
            refund.Status = PaymentRefundStatus.Processed;
            refund.GatewayRefundId = gatewayRefund.GatewayRefundId;
            refund.CompletedAt = DateTime.UtcNow;
            refund.LastCheckedAt = DateTime.UtcNow;

            payment.RefundedAmount = await _unitOfWork.PaymentRefunds.GetTotalRefundedAmountAsync(payment.Id, cancellationToken);
            payment.Status = payment.RefundedAmount >= payment.Amount ? PaymentStatus.Refunded : PaymentStatus.PartiallyRefunded;
            payment.ModifiedAt = DateTime.UtcNow;

            _unitOfWork.PaymentRefunds.Update(refund);
            _unitOfWork.Payments.Update(payment);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}

