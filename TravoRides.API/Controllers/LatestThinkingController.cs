using TravoRides.Application.Common.Responses;
using TravoRides.Application.DTOs.Common;
using TravoRides.Application.DTOs.LatestThinking;
using TravoRides.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace TravoRides.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LatestThinkingController : ControllerBase
    {
        private readonly ILatestThinkingService _service;

        public LatestThinkingController(ILatestThinkingService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<PagedResponse<LatestThinkingDTO>>>> GetAll([FromQuery] SearchLatestThinkingRequest request, CancellationToken cancellationToken)
        {
            var response = await _service.GetAllAsync(request, cancellationToken);

            return Ok(new ApiResponse<PagedResponse<LatestThinkingDTO>>
            {
                IsSuccess = true,
                Message = "Latest thinking retrieved successfully.",
                Data = response
            });
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken = default)
        {
            var item = await _service.GetByIdAsync(id, cancellationToken);

            if (item == null)
                return NotFound(new ApiResponse<object>
                {
                    IsSuccess = false,
                    Message = "Latest thinking not found.",
                    Data = null
                });

            return Ok(new ApiResponse<object>
            {
                IsSuccess = true,
                Message = "Latest thinking retrieved successfully.",
                Data = item
            });
        }

        [HttpPost]
       // [Authorize]
        public async Task<IActionResult> Create([FromForm] CreateLatestThinkingRequest request, CancellationToken cancellationToken = default)
        {
            var id = await _service.CreateAsync(request, cancellationToken);

            return CreatedAtAction(nameof(GetById), new { id }, new ApiResponse<object>
            {
                IsSuccess = true,
                Message = "Latest thinking created successfully.",
                Data = id
            });
        }

        [HttpPut("{id:guid}")]
      //  [Authorize]
        public async Task<IActionResult> Update(Guid id, [FromForm] UpdateLatestThinkingRequest request, CancellationToken cancellationToken = default)
        {
            request.Id = id;

            await _service.UpdateAsync(request, cancellationToken);

            return Ok(new ApiResponse<object>
            {
                IsSuccess = true,
                Message = "Latest thinking updated successfully.",
                Data = id
            });
        }

        [HttpDelete("{id:guid}")]
      //  [Authorize]
        public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken = default)
        {
            await _service.DeleteAsync(id, cancellationToken);

            return Ok(new ApiResponse<object>
            {
                IsSuccess = true,
                Message = "Latest thinking deleted successfully.",
                Data = id
            });
        }
    }
}
