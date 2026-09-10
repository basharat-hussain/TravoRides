using TravoRides.Application.Common.Responses;
using TravoRides.Application.DTOs.Common;
using TravoRides.Application.DTOs.Review;
using TravoRides.Application.DTOs.Subscription;
using TravoRides.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace TravoRides.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SubscriptionController : ControllerBase
    {
        private readonly ISubscriptionService _service;
        public SubscriptionController(ISubscriptionService service)
        {
            _service = service;
        }

        [HttpGet]
      //  [Authorize]
        public async Task<IActionResult> GetAll([FromQuery] SearchSubscriptionRequest request, CancellationToken cancellationToken = default)
        {
            var subscription = await _service.GetAllAsync(request, cancellationToken);

            return Ok(new ApiResponse<PagedResponse<SubscriptionDTO>>
            {
                IsSuccess = true,
                Message = "Subscription retrieved successfully.",
                Data = subscription
            });
        }       

        // GET: api/subscription/{id}
        [HttpGet("{id:guid}")]
       // [Authorize]
        public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken = default)
        {
            var enquiry = await _service.GetByIdAsync(id, cancellationToken);

            if (enquiry == null)
                return NotFound(new ApiResponse<object>
                {
                    IsSuccess = false,
                    Message = "Subscription not found.",
                    Data = null
                });

            return Ok(new ApiResponse<object>
            {
                IsSuccess = true,
                Message = "Subscription retrieved successfully.",
                Data = enquiry
            });
        }

        // POST: api/subscription
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateSubscriptionRequest request, CancellationToken cancellationToken = default)
        {
            var id = await _service.CreateAsync(request, cancellationToken);

            return CreatedAtAction(nameof(GetById), new { id }, new ApiResponse<object>
            {
                IsSuccess = true,
                Message = "Subscription created successfully.",
                Data = id
            });
        }

   

        // DELETE: api/subscription/{id}
        [HttpDelete("{id:guid}")]
        [Authorize]
        public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken = default)
        {
            await _service.DeleteAsync(id, cancellationToken);

            return Ok(new ApiResponse<object>
            {
                IsSuccess = true,
                Message = "Subscription deleted successfully.",
                Data = id
            });
        }
    }
}
