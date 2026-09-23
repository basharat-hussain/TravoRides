using Microsoft.Extensions.Options;
using Razorpay.Api;
using TravoRides.Application.Common.Options;
using TravoRides.Application.DTOs.Payment;
using TravoRides.Application.Interfaces.Payment;

namespace TravoRides.Infrastructure.Payments.Razorpay
{
    public class RazorpayPaymentGateway : IPaymentGateway
    {
        public string GatewayName => "Razorpay";
        public string PublicKey => _options.KeyId;
        private readonly RazorpayOptions _options;
        private readonly RazorpayClient _client;

        public RazorpayPaymentGateway(IOptions<RazorpayOptions> options, RazorpayClient client)
        {
            _options = options.Value;
            _client = client;
        }

        public Task<PaymentGatewayOrderResult> CreateOrderAsync(PaymentGatewayOrderRequest request, CancellationToken cancellationToken = default)
        {
            try
            {
                var amountInPaise = ConvertToSubUnits(request.Amount);

                var input = new Dictionary<string, object>
                {
                    { "amount", amountInPaise },
                    { "currency", request.Currency },
                    { "receipt", request.PaymentNumber }
                };

                var order = _client.Order.Create(input);

                var result = new PaymentGatewayOrderResult
                {
                    Success = true,
                    GatewayOrderId = order["id"]?.ToString(),
                    GatewayName = "Razorpay"
                };
                return Task.FromResult(result);
            }
            catch (Exception ex)
            {
                var result = new PaymentGatewayOrderResult
                {
                    Success = false,
                    GatewayName = "Razorpay",
                    ErrorMessage = ex.Message
                };
                return Task.FromResult(result);
            }
        }

        public Task<PaymentGatewayVerificationResult> VerifyPaymentAsync(PaymentGatewayVerificationRequest request, CancellationToken cancellationToken = default)
        {
            try
            {
                var attributes = new Dictionary<string, string>
                {
                    { "razorpay_order_id", request.GatewayOrderId },
                    { "razorpay_payment_id", request.GatewayTransactionId },
                    { "razorpay_signature", request.Signature }
                };

                Utils.verifyPaymentSignature(attributes);

                global::Razorpay.Api.Payment payment = _client.Payment.Fetch(request.GatewayTransactionId);

                string currency = payment["currency"].ToString();
                decimal amount = ConvertFromSubUnits(payment["amount"]);
                string status = payment["status"].ToString();

                return Task.FromResult(new PaymentGatewayVerificationResult
                {
                    Success = true,
                    IsCaptured = true,
                    GatewayTransactionId = request.GatewayTransactionId,
                    Amount = amount,
                    Currency = currency,
                });
            }
            catch (Exception ex)
            {
                return Task.FromResult(new PaymentGatewayVerificationResult
                {
                    Success = false,
                    ErrorMessage = ex.Message
                });
            }
        }

        public Task<PaymentGatewayRefundResult> RefundAsync(PaymentGatewayRefundRequest request, CancellationToken cancellationToken = default)
        {
            try
            {
                var amountInPaise = ConvertToSubUnits(request.Amount);

                var input = new Dictionary<string, object>
                {
                    { "amount", amountInPaise }
                };

                var refund = _client.Payment.Fetch(request.GatewayTransactionId).Refund(input);

                return Task.FromResult(new PaymentGatewayRefundResult
                {
                    GatewayRefundId = refund["id"]?.ToString(),
                    Amount = ConvertFromSubUnits(refund["amount"]),
                    Status = Domain.Enums.PaymentGatewayResultStatus.Success
                });
            }
            catch (Exception ex)
            {
                return Task.FromResult(new PaymentGatewayRefundResult
                {
                    Status = Domain.Enums.PaymentGatewayResultStatus.Failed,
                    ErrorMessage = ex.Message
                });
            }
        }

        public Task<IReadOnlyList<PaymentGatewayRefundResult>> GetRefundsAsync(string gatewayTransactionId, CancellationToken cancellationToken)
        {
            var refunds = _client.Payment
                .Fetch(gatewayTransactionId)
                .AllRefunds();

            IReadOnlyList<PaymentGatewayRefundResult> result = refunds.Select(r => new PaymentGatewayRefundResult
            {
                GatewayRefundId = r["id"]?.ToString() ?? string.Empty,
                Amount = ConvertFromSubUnits(r["amount"]),
                Status = Domain.Enums.PaymentGatewayResultStatus.Success
            }).ToList();

            return Task.FromResult(result);
        }

        private static long ConvertToSubUnits(decimal amount)
        {
            return checked((long)Math.Round(amount * 100m, 0, MidpointRounding.AwayFromZero));
        }

        private static decimal ConvertFromSubUnits(object? value)
        {
            if (value == null)
                return 0m;

            return Convert.ToDecimal(value) / 100m;
        }
    }
}
