using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Hangfire;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using TravoRides.Application.Common.Exceptions;
using TravoRides.Application.DTOs.Payment;
using TravoRides.Application.Interfaces;
using TravoRides.Application.Interfaces.Notifications;
using TravoRides.Application.Interfaces.Services;
using TravoRides.Application.Repositories;
using TravoRides.Domain.Entities;
using TravoRides.Domain.Enums;

namespace TravoRides.Infrastructure.Payments.Razorpay
{
    public class RazorpayPaymentWebhookService : IRazorPayPaymentWebhookService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfiguration _configuration;
        private readonly IPaymentStateService _paymentStateService;
        private readonly IJobScheduler _jobScheduler;

        public RazorpayPaymentWebhookService(
            IUnitOfWork unitOfWork,
            IConfiguration configuration,
            IPaymentStateService paymentStateService,
            IJobScheduler jobScheduler)
        {
            _unitOfWork = unitOfWork;
            _configuration = configuration;
            _paymentStateService = paymentStateService;
            _jobScheduler = jobScheduler;
        }

        public async Task ReceiveWebhookAsync(string rawPayload, string signature, string eventId, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(rawPayload))
                throw new ValidationException("Webhook payload cannot be empty.");

            if (string.IsNullOrWhiteSpace(signature))
                throw new ValidationException("Webhook signature is missing.");

            if (string.IsNullOrWhiteSpace(eventId))
                throw new ValidationException("Webhook event ID is missing.");

            var webHookSecret = _configuration["PaymentGateway:Razorpay:WebhookSecret"];

            if (string.IsNullOrWhiteSpace(webHookSecret))
                throw new InvalidOperationException("Razorpay webhook secret is not configured.");

            var signatureValid = IsValidSignature(rawPayload, signature, webHookSecret);

            if (!signatureValid)
                throw new UnauthorizedAccessException("Invalid Razorpay webhook signature.");

            var webhookRequest = JsonSerializer.Deserialize<RazorpayWebhookRequest>(rawPayload, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (webhookRequest == null)
                throw new ValidationException("Invalid Razorpay webhook payload.");

            if (string.IsNullOrWhiteSpace(webhookRequest.Event))
                throw new ValidationException("Webhook event type is missing.");

            if (webhookRequest.CreatedAt <= 0)
                throw new ValidationException("Webhook created_at is missing.");

            var eventCreatedAt = DateTimeOffset.FromUnixTimeSeconds(webhookRequest.CreatedAt);
            var age = DateTimeOffset.UtcNow - eventCreatedAt;

            if (age > TimeSpan.FromMinutes(5))
                throw new ValidationException("Webhook event is too old.");

            if (eventCreatedAt > DateTimeOffset.UtcNow.AddMinutes(5))
                throw new ValidationException("Webhook event timestamp is invalid.");

            var supportedEvents = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "payment.captured", "payment.failed" };

            if (!supportedEvents.Contains(webhookRequest.Event))
                return;

            var existingWebhook = await _unitOfWork.PaymentWebhooks.GetByEventIdAsync(eventId, cancellationToken);
            if (existingWebhook != null)
            {
                return;
            }

            var webhook = new PaymentWebhook
            {
                EventId = eventId,
                EventType = webhookRequest.Event,
                Payload = rawPayload,
                IsProcessed = false,
                ReceivedAt = DateTime.UtcNow,
                PaymentGatewayId = webhookRequest.Payload?.Payment?.Entity?.Id,
                GatewayOrderId = webhookRequest.Payload?.Payment?.Entity?.OrderId,
                RazorpayCreatedAt = DateTime.UtcNow,
                HangfireJobId = string.Empty
            };

            try
            {
                await _unitOfWork.PaymentWebhooks.AddAsync(webhook, cancellationToken);
                await _unitOfWork.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateException)
            {
                var existing = await _unitOfWork.PaymentWebhooks.GetByEventIdAsync(eventId, cancellationToken);
                if (existing != null)
                    return;

                throw;
            }

            var jobId = _jobScheduler.Enqueue<IRazorPayPaymentWebhookService>(service =>
                service.ProcessWebhookAsync(webhookRequest, eventId, signature, rawPayload, CancellationToken.None));

            webhook.HangfireJobId = jobId;
            _unitOfWork.PaymentWebhooks.Update(webhook);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        [AutomaticRetry(Attempts = 5, DelaysInSeconds = new[] { 60, 300, 900, 1800, 3600 }, OnAttemptsExceeded = AttemptsExceededAction.Fail)]
        public async Task ProcessWebhookAsync(RazorpayWebhookRequest request, string eventId, string signature, string rawPayload, CancellationToken cancellationToken = default)
        {
            if (request == null)
                throw new ValidationException("Webhook request cannot be null.");

            if (string.IsNullOrWhiteSpace(eventId))
                throw new ValidationException("Razorpay webhook event ID is missing.");

            if (string.IsNullOrWhiteSpace(signature))
                throw new UnauthorizedAccessException("Razorpay webhook signature is missing.");

            if (string.IsNullOrWhiteSpace(rawPayload))
                throw new ValidationException("Razorpay webhook payload is empty.");

            await _unitOfWork.BeginTransactionAsync(cancellationToken);

            try
            {
                var webhook = await _unitOfWork.PaymentWebhooks.GetForProcessingAsync(eventId, cancellationToken);

                if (webhook == null)
                    throw new ResourceNotFoundException("Payment webhook record not found.");

                if (webhook.IsProcessed)
                {
                    await _unitOfWork.CommitTransactionAsync(cancellationToken);
                    return;
                }

                Booking? booking = null;

                switch (request.Event)
                {
                    case "payment.captured":
                        booking = await ProcessPaymentCapturedAsync(webhook, request, cancellationToken);
                        break;

                    case "payment.failed":
                        await ProcessPaymentFailedAsync(webhook, request, cancellationToken);
                        break;

                    default:
                        throw new ConflictException($"Unsupported Razorpay webhook event: {request.Event}");
                }

                await _unitOfWork.SaveChangesAsync(cancellationToken);
                await _unitOfWork.CommitTransactionAsync(cancellationToken);

                if (request.Event == "payment.captured" && booking != null)
                {
                    _jobScheduler.Enqueue<IBookingNotificationService>(x => x.SendBookingConfirmationAsync(booking.Id, CancellationToken.None));
                }
            }
            catch
            {
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                throw;
            }
        }

        private async Task<Booking> ProcessPaymentCapturedAsync(PaymentWebhook webhook, RazorpayWebhookRequest request, CancellationToken cancellationToken)
        {
            var paymentEntity = request.Payload?.Payment?.Entity;

            if (paymentEntity == null)
                throw new ValidationException("Payment information is missing from Razorpay webhook.");

            if (string.IsNullOrWhiteSpace(paymentEntity.Id))
                throw new ValidationException("Razorpay payment ID is missing.");

            if (string.IsNullOrWhiteSpace(paymentEntity.OrderId))
                throw new ValidationException("Razorpay order ID is missing.");

            if (!paymentEntity.Captured || !string.Equals(paymentEntity.Status, "captured", StringComparison.OrdinalIgnoreCase))
                throw new ConflictException("Razorpay payment has not been captured.");

            var payment = await _unitOfWork.Payments.GetByGatewayOrderIdAsync(paymentEntity.OrderId, cancellationToken);

            if (payment == null)
                throw new ResourceNotFoundException("Payment associated with Razorpay order was not found.");

            var booking = await _unitOfWork.Bookings.GetByIdAsync(payment.BookingId, cancellationToken);
            if (booking == null)
                throw new ResourceNotFoundException("Associated booking was not found.");

            if (payment.Status == PaymentStatus.Paid)
            {
                webhook.IsProcessed = true;
                webhook.ProcessedAt = DateTime.UtcNow;
                _unitOfWork.PaymentWebhooks.Update(webhook);
                await _unitOfWork.SaveChangesAsync(cancellationToken);
                return booking;
            }

            var existingPayment = await _unitOfWork.Payments.GetByGatewayTransactionIdAsync(paymentEntity.Id, cancellationToken);
            if (existingPayment != null && existingPayment.Id != payment.Id)
                throw new ConflictException("Razorpay payment transaction has already been processed.");

            var webhookAmount = paymentEntity.Amount / 100m;

            if (decimal.Round(webhookAmount, 2) != decimal.Round(payment.Amount, 2))
                throw new ConflictException("Payment amount does not match.");

            if (!string.Equals(paymentEntity.Currency, payment.Currency, StringComparison.OrdinalIgnoreCase))
                throw new ConflictException("Payment currency does not match.");

            await _paymentStateService.MarkPaymentAsPaidAsync(payment, booking, paymentEntity.Id, cancellationToken);

            webhook.IsProcessed = true;
            webhook.ProcessedAt = DateTime.UtcNow;
            webhook.ErrorMessage = null;

            _unitOfWork.PaymentWebhooks.Update(webhook);

            return booking;
        }

        private async Task ProcessPaymentFailedAsync(PaymentWebhook webhook, RazorpayWebhookRequest request, CancellationToken cancellationToken)
        {
            var paymentEntity = request.Payload?.Payment?.Entity;

            if (paymentEntity == null)
                throw new ValidationException("Payment information is missing from Razorpay webhook.");

            if (string.IsNullOrWhiteSpace(paymentEntity.Id))
                throw new ValidationException("Razorpay payment ID is missing.");

            if (string.IsNullOrWhiteSpace(paymentEntity.OrderId))
                throw new ValidationException("Razorpay order ID is missing.");

            var payment = await _unitOfWork.Payments.GetByGatewayOrderIdAsync(paymentEntity.OrderId, cancellationToken);

            if (payment == null)
                throw new ResourceNotFoundException("Payment associated with Razorpay order was not found.");

            var booking = await _unitOfWork.Bookings.GetByIdAsync(payment.BookingId, cancellationToken);

            if (booking == null)
                throw new ResourceNotFoundException("Associated booking was not found.");

            if (payment.Status == PaymentStatus.Paid)
            {
                webhook.IsProcessed = true;
                webhook.ProcessedAt = DateTime.UtcNow;
                _unitOfWork.PaymentWebhooks.Update(webhook);
                await _unitOfWork.SaveChangesAsync(cancellationToken);
                return;
            }

            var existingPayment = await _unitOfWork.Payments.GetByGatewayTransactionIdAsync(paymentEntity.Id, cancellationToken);

            if (existingPayment != null && existingPayment.Id != payment.Id)
                throw new ConflictException("Razorpay payment transaction has already been processed.");

            var webhookAmount = paymentEntity.Amount / 100m;

            if (decimal.Round(webhookAmount, 2) != decimal.Round(payment.Amount, 2))
                throw new ConflictException("Payment amount does not match.");

            if (!string.Equals(paymentEntity.Currency, payment.Currency, StringComparison.OrdinalIgnoreCase))
                throw new ConflictException("Payment currency does not match.");

            payment.Status = PaymentStatus.Failed;
            payment.GatewayTransactionId = paymentEntity.Id;
            payment.FailedAt = DateTime.UtcNow;
            payment.FailureReason = paymentEntity.ErrorDescription ?? paymentEntity.ErrorReason ?? paymentEntity.ErrorCode ?? "Payment failed.";
            payment.ModifiedAt = DateTime.UtcNow;

            webhook.IsProcessed = true;
            webhook.ProcessedAt = DateTime.UtcNow;
            webhook.ErrorMessage = null;

            _unitOfWork.Payments.Update(payment);
            _unitOfWork.PaymentWebhooks.Update(webhook);
        }

        private static bool IsValidSignature(string rawPayload, string receivedSignature, string webhookSecret)
        {
            if (string.IsNullOrWhiteSpace(rawPayload) || string.IsNullOrWhiteSpace(receivedSignature))
                return false;

            using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(webhookSecret));
            var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(rawPayload));
            var expectedSignature = Convert.ToHexString(hash).ToLowerInvariant();

            return CryptographicOperations.FixedTimeEquals(
                Encoding.UTF8.GetBytes(expectedSignature),
                Encoding.UTF8.GetBytes(receivedSignature.ToLowerInvariant()));
        }
    }
}

