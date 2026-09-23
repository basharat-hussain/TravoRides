using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TravoRides.Application.Interfaces.Services;

namespace TravoRides.API.Controllers
{
    [ApiController]
    [Route("api/payments/webhook")]
    [AllowAnonymous]
    public class PaymentWebhookController : ControllerBase
    {
        private readonly IRazorPayPaymentWebhookService _paymentWebhookService;

        public PaymentWebhookController(IRazorPayPaymentWebhookService paymentWebhookService)
        {
            _paymentWebhookService = paymentWebhookService;
        }

        [HttpPost]
        public async Task<IActionResult> Handle(CancellationToken cancellationToken)
        {
            Request.EnableBuffering();

            using var reader = new StreamReader(Request.Body, Encoding.UTF8, detectEncodingFromByteOrderMarks: false, leaveOpen: true);
            var rawPayload = await reader.ReadToEndAsync(cancellationToken);
            Request.Body.Position = 0;

            if (string.IsNullOrWhiteSpace(rawPayload))
                return BadRequest("Missing Razorpay webhook payload.");

            var signature = Request.Headers["X-Razorpay-Signature"].FirstOrDefault();
            var eventId = Request.Headers["X-Razorpay-Event-Id"].FirstOrDefault();

            if (string.IsNullOrWhiteSpace(signature))
                return BadRequest("Missing Razorpay webhook signature.");

            if (string.IsNullOrWhiteSpace(eventId))
                return BadRequest("Missing Razorpay webhook event ID.");

            await _paymentWebhookService.ReceiveWebhookAsync(rawPayload, signature, eventId, cancellationToken);

            return Ok(new
            {
                received = true
            });
        }
    }
}

