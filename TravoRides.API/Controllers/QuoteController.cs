using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using TravoRides.Application.Common.Responses;
using TravoRides.Application.DTOs.Common;
using TravoRides.Application.DTOs.Quote;
using TravoRides.Application.Interfaces;
using TravoRides.Domain.Enums;

namespace TravoRides.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class QuoteController : ControllerBase
    {
        private readonly IQuoteService _service;

        public QuoteController(IQuoteService service)
        {
            _service = service;
        }

        [HttpGet]
        [Authorize(Roles = nameof(UserRole.Admin))]
        [EnableRateLimiting("generic-api")]
        public async Task<IActionResult> GetAll([FromQuery] SearchQuoteRequest request, CancellationToken cancellationToken)
        {
            var data = await _service.GetAllAsync(request, cancellationToken);
            return Ok(new ApiResponse<PagedResponse<QuoteDTO>> { IsSuccess = true, Message = "Quotes retrieved successfully.", Data = data });
        }

        [HttpGet("{id:guid}")]
        [Authorize(Roles = nameof(UserRole.Admin))]
        [EnableRateLimiting("generic-api")]
        public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken = default)
        {
            var item = await _service.GetByIdAsync(id, cancellationToken);
            if (item == null)
                return NotFound(new ApiResponse<object> { IsSuccess = false, Message = "Quote not found.", Data = null });

            return Ok(new ApiResponse<object> { IsSuccess = true, Message = "Quote retrieved successfully.", Data = item });
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateQuoteRequest request, CancellationToken cancellationToken = default)
        {
            var id = await _service.CreateAsync(request, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id }, new ApiResponse<object> { IsSuccess = true, Message = "Quote created successfully.", Data = id });
        }

        [HttpPut("{id:guid}")]
        [Authorize(Roles = nameof(UserRole.Admin))]
        [EnableRateLimiting("generic-api")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateQuoteRequest request, CancellationToken cancellationToken = default)
        {
            request.Id = id;
            await _service.UpdateAsync(request, cancellationToken);
            return Ok(new ApiResponse<object> { IsSuccess = true, Message = "Quote updated successfully.", Data = id });
        }

        [HttpDelete("{id:guid}")]
        [Authorize(Roles = nameof(UserRole.Admin))]
        [EnableRateLimiting("generic-api")]
        public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken = default)
        {
            await _service.DeleteAsync(id, cancellationToken);
            return Ok(new ApiResponse<object> { IsSuccess = true, Message = "Quote deleted successfully.", Data = id });
        }
    }
}
