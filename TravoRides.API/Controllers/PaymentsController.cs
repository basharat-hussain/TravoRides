using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using TravoRides.Application.Common.Responses;
using TravoRides.Application.DTOs.Payment;
using TravoRides.Application.Interfaces;
using TravoRides.Domain.Enums;

namespace TravoRides.API.Controllers
{
    [Route("api/payments")]
    [ApiController]
    public class PaymentsController : ControllerBase
    {
        private readonly IPaymentService _paymentService;
        private readonly IPaymentRefundService _paymentRefundService;

        public PaymentsController(IPaymentService paymentService, IPaymentRefundService paymentRefundService)
        {
            _paymentService = paymentService;
            _paymentRefundService = paymentRefundService;
        }

        [HttpPost]
        [EnableRateLimiting("payment-api")]
        public async Task<IActionResult> CreatePayment([FromBody] CreatePaymentRequest request, CancellationToken cancellationToken)
        {
            var result = await _paymentService.CreatePaymentAsync(request, cancellationToken);

            return Ok(new ApiResponse<CreatePaymentResponse>
            {
                IsSuccess = true,
                Message = "Payment created successfully",
                Data = result
            });
        }

        [HttpPost("{paymentId:guid}/verify")]
        [EnableRateLimiting("payment-api")]
        public async Task<IActionResult> VerifyPayment(Guid paymentId, [FromBody] VerifyPaymentRequest request, CancellationToken cancellationToken)
        {
            var result = await _paymentService.VerifyPaymentAsync(paymentId, request, cancellationToken);

            return Ok(new ApiResponse<PaymentVerificationResponse>
            {
                IsSuccess = true,
                Message = "Payment verified successfully",
                Data = result
            });
        }

        [Authorize(Roles = nameof(UserRole.Admin))]
        [HttpPost("{paymentId:guid}/refund")]
        [EnableRateLimiting("payment-api")]
        public async Task<IActionResult> RefundPayment(Guid paymentId, [FromBody] RefundPaymentRequest request, CancellationToken cancellationToken)
        {
            var idempotencyKey = Request.Headers["Idempotency-Key"].FirstOrDefault();

            if (string.IsNullOrWhiteSpace(idempotencyKey))
            {
                return BadRequest("Idempotency-Key header is required.");
            }

            var result = await _paymentRefundService.RefundPaymentAsync(paymentId, request, idempotencyKey, cancellationToken);

            return Ok(new ApiResponse<RefundPaymentResponse>
            {
                IsSuccess = true,
                Message = "Payment refunded successfully",
                Data = result
            });
        }

        [HttpGet("{paymentId:guid}/refunds")]
        [Authorize(Roles = nameof(UserRole.Admin))]
        public async Task<IActionResult> GetRefunds(Guid paymentId, CancellationToken cancellationToken)
        {
            var refunds = await _paymentRefundService.GetRefundsAsync(paymentId, cancellationToken);

            return Ok(new ApiResponse<IReadOnlyList<RefundPaymentResponse>>
            {
                IsSuccess = true,
                Message = "Refunds retrieved successfully",
                Data = refunds
            });
        }
    }
}

