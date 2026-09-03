using TravoRiders.Application.Common.Responses;
using TravoRiders.Application.DTOs.Common;
using TravoRiders.Application.DTOs.Quote;
using TravoRiders.Application.DTOs.Review;
using TravoRiders.Application.Interfaces;
using TravoRiders.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace TravoRiders.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReviewController : ControllerBase
    {
        private readonly IReviewService _service;

        public ReviewController(IReviewService service)
        {
            this._service = service;
        }


        [HttpGet("Admin")]
        [Authorize]
        public async Task<IActionResult> GetAll([FromQuery] SearchReviewRequest request, CancellationToken cancellationToken = default)
        {
            var review = await _service.GetAllAsync(request, cancellationToken);

            return Ok(new ApiResponse<PagedResponse<ReviewDTO>>
            {
                IsSuccess = true,
                Message = "Review retrieved successfully.",
                Data = review
            });
        }

        [HttpGet("User")]
        public async Task<IActionResult> GetAllApproved([FromQuery]SearchReviewRequest request, CancellationToken cancellationToken = default)
        {
            var review = await _service.GetAllApprovedAsync(request, cancellationToken);

            return Ok(new ApiResponse<PagedResponse<ReviewDTO>>
            {
                IsSuccess = true,
                Message = "Approved reviews retrieved successfully.",
                Data = review
            });
        }

        // GET: api/Review/{id}
        [HttpGet("{id:guid}")]
        [Authorize]
        public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken = default)
        {
            var review = await _service.GetByIdAsync(id, cancellationToken);

            if (review == null)
                return NotFound(new ApiResponse<object>
                {
                    IsSuccess = false,
                    Message = "Review not found.",
                    Data = null
                });

            return Ok(new ApiResponse<object>
            {
                IsSuccess = true,
                Message = "Review retrieved successfully.",
                Data = review
            });
        }

        // POST: api/Review
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateReviewRequest request, CancellationToken cancellationToken = default)
        {
            var id = await _service.CreateAsync(request, cancellationToken);

            return CreatedAtAction(nameof(GetById), new { id }, new ApiResponse<object>
            {
                IsSuccess = true,
                Message = "Review created successfully.",
                Data = id
            });
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateReviewRequest request, CancellationToken cancellationToken = default)
        {
            request.Id = id;
            await _service.UpdateAsync(request, cancellationToken);
            return Ok(new ApiResponse<object> { IsSuccess = true, Message = "Review updated successfully.", Data = id });
        }

        [HttpPut("{id:guid}/status")]
        [Authorize]
        public async Task<IActionResult> UpdateStatus(Guid id, [FromBody] UpdateStatusRequest request, CancellationToken cancellationToken = default)
        {
            if (request == null)
                return BadRequest(new ApiResponse<object> { IsSuccess = false, Message = "Invalid payload.", Data = null });

            await _service.ToggleStatusAsync(id, request.IsActive, cancellationToken);

            return Ok(new ApiResponse<object>
            {
                IsSuccess = true,
                Message = "Review status updated successfully.",
                Data = id
            });
        }


        // DELETE: api/Review/{id}
        [HttpDelete("{id:guid}")]
        [Authorize]
        public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken = default)
        {
            await _service.DeleteAsync(id, cancellationToken);

            return Ok(new ApiResponse<object>
            {
                IsSuccess = true,
                Message = "Review deleted successfully.",
                Data = id
            });
        }
    }
}
