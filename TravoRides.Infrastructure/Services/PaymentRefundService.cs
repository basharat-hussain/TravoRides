using Microsoft.EntityFrameworkCore;
using TravoRides.Application.Common.Exceptions;
using TravoRides.Application.DTOs.Payment;
using TravoRides.Application.Interfaces;
using TravoRides.Application.Interfaces.Payment;
using TravoRides.Application.Repositories;
using TravoRides.Domain.Entities;
using TravoRides.Domain.Enums;

namespace TravoRides.Infrastructure.Services
{
    public class PaymentRefundService : IPaymentRefundService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPaymentGateway _paymentGateway;

        public PaymentRefundService(IUnitOfWork unitOfWork, IPaymentGateway paymentGateway)
        {
            _unitOfWork = unitOfWork;
            _paymentGateway = paymentGateway;
        }

        public async Task<RefundPaymentResponse> RefundPaymentAsync(Guid paymentId, RefundPaymentRequest request, string idempotencyKey, CancellationToken cancellationToken = default)
        {
            var existingRefund = await _unitOfWork.PaymentRefunds.GetByIdempotencyKeyAsync(idempotencyKey, cancellationToken);
            if (existingRefund != null)
            {
                if (existingRefund.PaymentId != paymentId || existingRefund.Amount != request.Amount)
                    throw new ConflictException("The idempotency key was already used for a different refund request.");

                return BuildRefundResponse(existingRefund);
            }

            if (request.Amount <= 0)
                throw new ConflictException("Refund amount must be greater than zero.");

            request.Amount = decimal.Round(request.Amount, 2, MidpointRounding.AwayFromZero);

            if (string.IsNullOrWhiteSpace(idempotencyKey))
                throw new ValidationException("Idempotency key is required.");

            if (idempotencyKey.Length > 100)
                throw new ValidationException("Invalid idempotency key.");

            var payment = await _unitOfWork.Payments.GetByIdAsync(paymentId, cancellationToken);
            if (payment == null)
                throw new ResourceNotFoundException("Payment not found.");

            if (payment.Status != PaymentStatus.Paid && payment.Status != PaymentStatus.PartiallyRefunded)
                throw new ConflictException("Payment is not eligible for refund.");

            if (string.IsNullOrWhiteSpace(payment.GatewayTransactionId))
                throw new ConflictException("Gateway transaction ID is missing.");

            var pendingRefund = await _unitOfWork.PaymentRefunds.GetPendingByPaymentIdAsync(paymentId, cancellationToken);
            if (pendingRefund != null)
                throw new ConflictException("A refund for this payment is already being processed.");

            var refundedAmount = await _unitOfWork.PaymentRefunds.GetTotalRefundedAmountAsync(paymentId, cancellationToken);
            var refundableAmount = payment.Amount - refundedAmount;

            if (request.Amount > refundableAmount)
                throw new ConflictException($"Refund amount exceeds the refundable amount of {refundableAmount:0.00}.");

            var refund = new PaymentRefund
            {
                PaymentId = payment.Id,
                Amount = request.Amount,
                Currency = payment.Currency,
                Status = PaymentRefundStatus.Pending,
                Reason = request.Reason,
                CreatedAt = DateTime.UtcNow,
                RefundNumber = $"RFND-{Guid.NewGuid()}",
                IdempotencyKey = idempotencyKey
            };

            try
            {
                await _unitOfWork.PaymentRefunds.AddAsync(refund, cancellationToken);
                await _unitOfWork.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateException)
            {
                var existing = await _unitOfWork.PaymentRefunds.GetByIdempotencyKeyAsync(idempotencyKey, cancellationToken);
                if (existing != null)
                {
                    return BuildRefundResponse(existing);
                }

                throw;
            }

            var result = await _paymentGateway.RefundAsync(new PaymentGatewayRefundRequest
            {
                GatewayTransactionId = payment.GatewayTransactionId,
                Amount = request.Amount,
                Currency = payment.Currency,
                Reason = request.Reason
            }, cancellationToken);

            if (result.Status == PaymentGatewayResultStatus.Failed)
            {
                refund.Status = PaymentRefundStatus.Failed;
                refund.FailureReason = result.ErrorMessage ?? "Gateway refund failed.";

                _unitOfWork.PaymentRefunds.Update(refund);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                throw new ConflictException(refund.FailureReason);
            }

            if (string.IsNullOrWhiteSpace(result.GatewayRefundId))
                throw new ConflictException("Gateway did not return a refund ID.");

            if (result.Amount.HasValue && decimal.Round(result.Amount.Value, 2) != request.Amount)
                throw new ConflictException("Gateway refund amount does not match.");

            if (!string.Equals(result.Currency, payment.Currency, StringComparison.OrdinalIgnoreCase))
                throw new ConflictException("Gateway refund currency does not match.");

            var totalRefund = await _unitOfWork.PaymentRefunds.GetTotalRefundedAmountAsync(payment.Id, cancellationToken);

            refund.Status = PaymentRefundStatus.Processed;
            refund.GatewayRefundId = result.GatewayRefundId;
            refund.CompletedAt = DateTime.UtcNow;

            payment.RefundedAmount = totalRefund;
            payment.Status = payment.RefundedAmount >= payment.Amount ? PaymentStatus.Refunded : PaymentStatus.PartiallyRefunded;
            payment.ModifiedAt = DateTime.UtcNow;

            _unitOfWork.PaymentRefunds.Update(refund);
            _unitOfWork.Payments.Update(payment);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return BuildRefundResponse(refund);
        }

        private static RefundPaymentResponse BuildRefundResponse(PaymentRefund refund)
        {
            return new RefundPaymentResponse
            {
                PaymentId = refund.PaymentId,
                Success = refund.Status == PaymentRefundStatus.Processed,
                Status = refund.Status,
                RefundedAmount = refund.Amount,
                GatewayRefundId = refund.GatewayRefundId,
                Message = refund.Status switch
                {
                    PaymentRefundStatus.Processed => "Refund processed successfully.",
                    PaymentRefundStatus.Pending => "Refund is currently being processed.",
                    PaymentRefundStatus.Failed => refund.FailureReason ?? "Refund failed.",
                    _ => "Refund status is unknown."
                }
            };
        }

        public async Task<IReadOnlyList<RefundPaymentResponse>> GetRefundsAsync(Guid paymentId, CancellationToken cancellationToken = default)
        {
            var payment = await _unitOfWork.Payments.GetByIdAsync(paymentId, cancellationToken);

            if (payment == null)
                throw new ConflictException("Payment not found.");

            var refunds = await _unitOfWork.PaymentRefunds.GetByPaymentIdAsync(paymentId, cancellationToken);
            return refunds.Select(BuildRefundResponse).ToList();
        }
    }
}

