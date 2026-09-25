using AutoMapper;
using Microsoft.EntityFrameworkCore;
using TravoRides.Application.Common.Exceptions;
using TravoRides.Application.DTOs.Common;
using TravoRides.Application.DTOs.Payment;
using TravoRides.Application.Interfaces;
using TravoRides.Application.Interfaces.Notifications;
using TravoRides.Application.Interfaces.Payment;
using TravoRides.Application.Interfaces.Services;
using TravoRides.Application.Repositories;
using TravoRides.Domain.Entities;
using TravoRides.Domain.Enums;

namespace TravoRides.Application.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly IPaymentGateway _paymentGateway;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPaymentNumberGenerator _numberGenerator;
        private readonly IPaymentStateService _paymentStateService;
        private readonly IJobScheduler _jobScheduler;
        private readonly IMapper _mapper;

        public PaymentService(
            IPaymentGateway paymentGateway,
            IUnitOfWork unitOfWork,
            IPaymentNumberGenerator numberGenerator,
            IPaymentStateService paymentStateService,
            IJobScheduler jobScheduler,
            IMapper mapper)
        {
            _paymentGateway = paymentGateway;
            _unitOfWork = unitOfWork;
            _numberGenerator = numberGenerator;
            _paymentStateService = paymentStateService;
            _jobScheduler = jobScheduler;
            _mapper = mapper;
        }

        public async Task<CreatePaymentResponse> CreatePaymentAsync(CreatePaymentRequest request, CancellationToken cancellationToken = default)
        {
            var booking = await _unitOfWork.Bookings.GetByIdAsync(request.BookingId, cancellationToken);

            if (booking == null)
                throw new ResourceNotFoundException("Booking not found.");

            if (booking.IsConfirmed)
                throw new ConflictException("Booking has already been confirmed.");

            if (booking.TotalAmount <= 0)
                throw new ConflictException("Invalid booking rate.");

            var existingPayments = await _unitOfWork.Payments.GetByBookingIdAsync(booking.Id, cancellationToken);

            var successfulPayment = existingPayments.FirstOrDefault(x => x.Status == PaymentStatus.Paid);

            if (successfulPayment != null)
                throw new ConflictException("Payment has already been completed.");

            var pendingPayment = existingPayments.FirstOrDefault(x => x.Status == PaymentStatus.Pending);

            if (pendingPayment != null)
                return await BuildExistingPaymentResponseAsync(pendingPayment, cancellationToken);

            var attemptNumber = await _unitOfWork.Payments.GetNextAttemptNumberAsync(booking.Id, cancellationToken);
            var paymentNumber = await _numberGenerator.GenerateAsync(cancellationToken);

            var payment = new Payment
            {
                PaymentNumber = paymentNumber,
                AttemptNumber = attemptNumber,
                BookingId = booking.Id,
                Amount = booking.TotalAmount,
                Currency = "INR",
                Status = PaymentStatus.Pending,
                GatewayName = "Razorpay",
                RefundedAmount = 0,
                CreatedAt = DateTime.UtcNow,
                ModifiedAt = DateTime.UtcNow
            };

            try
            {
                await _unitOfWork.Payments.AddAsync(payment, cancellationToken);
                await _unitOfWork.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateException)
            {
                var existingPayment = await _unitOfWork.Payments.GetPendingByBookingIdAsync(booking.Id, cancellationToken);
                if (existingPayment != null)
                {
                    return await BuildExistingPaymentResponseAsync(existingPayment, cancellationToken);
                }

                throw;
            }

            var gatewayResult = await _paymentGateway.CreateOrderAsync(new PaymentGatewayOrderRequest
            {
                PaymentNumber = payment.PaymentNumber,
                Amount = payment.Amount,
                Currency = payment.Currency,
                Description = $"Ride booking #{booking.BookingNo}"
            }, cancellationToken);

            if (!gatewayResult.Success || string.IsNullOrWhiteSpace(gatewayResult.GatewayOrderId))
            {
                payment.Status = PaymentStatus.Failed;
                payment.FailedAt = DateTime.UtcNow;
                payment.FailureReason = gatewayResult.ErrorMessage;
                payment.ModifiedAt = DateTime.UtcNow;

                _unitOfWork.Payments.Update(payment);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                throw new InvalidOperationException(gatewayResult.ErrorMessage ?? "Unable to create payment order.");
            }

            payment.GatewayOrderId = gatewayResult.GatewayOrderId;
            payment.GatewayName = gatewayResult.GatewayName;
            payment.ModifiedAt = DateTime.UtcNow;

            _unitOfWork.Payments.Update(payment);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return new CreatePaymentResponse
            {
                PaymentId = payment.Id,
                PaymentNumber = payment.PaymentNumber,
                Amount = payment.Amount,
                Currency = payment.Currency,
                GatewayName = payment.GatewayName!,
                GatewayOrderId = payment.GatewayOrderId,
                GatewayKeyId = _paymentGateway.PublicKey
            };
        }

        private Task<CreatePaymentResponse> BuildExistingPaymentResponseAsync(Payment payment, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(payment.GatewayOrderId))
                throw new ConflictException("Existing payment does not have a gateway order.");

            return Task.FromResult(new CreatePaymentResponse
            {
                PaymentId = payment.Id,
                PaymentNumber = payment.PaymentNumber,
                Amount = payment.Amount,
                Currency = payment.Currency,
                GatewayName = payment.GatewayName ?? "Razorpay",
                GatewayOrderId = payment.GatewayOrderId,
                GatewayKeyId = _paymentGateway.PublicKey
            });
        }

        public async Task<PaymentVerificationResponse> VerifyPaymentAsync(Guid paymentId, VerifyPaymentRequest request, CancellationToken cancellationToken = default)
        {
            var payment = await _unitOfWork.Payments.GetByIdAsync(paymentId, cancellationToken);

            if (payment == null)
                throw new ResourceNotFoundException("Payment not found.");

            var booking = await _unitOfWork.Bookings.GetByIdAsync(payment.BookingId, cancellationToken);
            if (booking == null)
                throw new ResourceNotFoundException("Associated booking was not found.");

            if (payment.Status == PaymentStatus.Paid)
            {
                return new PaymentVerificationResponse
                {
                    PaymentId = payment.Id,
                    PaymentNumber = payment.PaymentNumber,
                    Status = payment.Status,
                    Success = true,
                    Message = "Payment has already been verified."
                };
            }

            if (string.IsNullOrWhiteSpace(payment.GatewayOrderId))
                throw new ConflictException("Payment gateway order is missing.");

            if (!string.Equals(payment.GatewayOrderId, request.GatewayOrderId, StringComparison.Ordinal))
                throw new ConflictException("Gateway order does not match.");

            var existingPayment = await _unitOfWork.Payments.GetByGatewayTransactionIdAsync(request.GatewayTransactionId, cancellationToken);
            if (existingPayment != null && existingPayment.Id != payment.Id)
                throw new ConflictException("Gateway transaction has already been processed.");

            var result = await _paymentGateway.VerifyPaymentAsync(new PaymentGatewayVerificationRequest
            {
                GatewayOrderId = request.GatewayOrderId,
                GatewayTransactionId = request.GatewayTransactionId,
                Signature = request.Signature
            }, cancellationToken);

            if (!result.Success)
                throw new ConflictException(result.ErrorMessage ?? "Payment verification failed.");

            if (!result.IsCaptured)
                throw new ConflictException("Payment has not been captured.");

            if (result.Amount.HasValue && decimal.Round(result.Amount.Value, 2) != decimal.Round(payment.Amount, 2))
                throw new ConflictException("Payment amount does not match.");

            if (!string.Equals(result.Currency, payment.Currency, StringComparison.OrdinalIgnoreCase))
                throw new ConflictException("Payment currency does not match.");

            await _paymentStateService.MarkPaymentAsPaidAsync(payment, booking, request.GatewayTransactionId, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _jobScheduler.Enqueue<IBookingNotificationService>(x => x.SendBookingConfirmationAsync(booking.Id, CancellationToken.None));

            return new PaymentVerificationResponse
            {
                PaymentId = payment.Id,
                PaymentNumber = payment.PaymentNumber,
                Status = payment.Status,
                Success = true,
                Message = "Payment verified successfully."
            };
        }

        public async Task<PagedResponse<PaymentDTO>> GetAllAsync(SearchPaymentRequest request, CancellationToken cancellationToken = default)
        {
            if (request.PageNumber < 1) request.PageNumber = 1;
            if (request.PageSize < 1) request.PageSize = 10;
            if (request.PageSize > 100) request.PageSize = 100;

            DateTime? fromDate = request.FromDate;
            DateTime? toDate = request.ToDate;

            if (!string.IsNullOrWhiteSpace(request.DateFilter))
            {
                var filter = request.DateFilter.Trim().ToLowerInvariant();
                var today = DateTime.Today;

                switch (filter)
                {
                    case "today":
                        fromDate = today;
                        toDate = today;
                        break;
                    case "thisweek":
                        var diff = (7 + (int)today.DayOfWeek - (int)DayOfWeek.Monday) % 7;
                        fromDate = today.AddDays(-diff);
                        toDate = today;
                        break;
                    case "thismonth":
                        fromDate = new DateTime(today.Year, today.Month, 1);
                        toDate = today;
                        break;
                    case "past3months":
                        fromDate = today.AddMonths(-3);
                        toDate = today;
                        break;
                    case "thisyear":
                        fromDate = new DateTime(today.Year, 1, 1);
                        toDate = today;
                        break;
                }
            }

            var paged = await _unitOfWork.Payments.GetAllSearchAsync(
                request.PageNumber,
                request.PageSize,
                request.Keyword,
                request.Status,
                fromDate,
                toDate,
                cancellationToken);

            var items = _mapper.Map<IEnumerable<PaymentDTO>>(paged.Items);

            return new PagedResponse<PaymentDTO>
            {
                Items = items,
                PageNumber = paged.PageNumber,
                PageSize = paged.PageSize,
                TotalCount = paged.TotalCount,
                TotalPages = paged.TotalPages
            };
        }

        public async Task<PaymentDTO?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var payment = await _unitOfWork.Payments.GetByIdWithBookingAsync(id, cancellationToken);
            if (payment == null) return null;
            return _mapper.Map<PaymentDTO>(payment);
        }
    }
}

