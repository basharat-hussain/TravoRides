using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TravoRiders.Application.Common.Responses;
using TravoRides.Application.DTOs.Cabs;
using TravoRides.Application.DTOs.Common;
using TravoRides.Application.Interfaces;

namespace TravoRides.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CabController : ControllerBase
    {
      private readonly ICabService _cabService;
        public CabController(ICabService cabService)
        {
            _cabService = cabService;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<PagedResponse<CabDTO>>>> GetAll(
      [FromQuery] SearchCategoryRequest request,
      CancellationToken cancellationToken)
        {
            var response = await _cabService.GetAllAsync(request, cancellationToken);

            return Ok(new ApiResponse<PagedResponse<CabDTO>>
            {
                IsSuccess = true,
                Message = "Cabs retrieved successfully.",
                Data = response
            });
        }
        // GET: api/Cab/{id}
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken = default)
        {
            var cabs = await _cabService.GetByIdAsync(id, cancellationToken);

            if (cabs == null)
                return NotFound(new ApiResponse<object>
                {
                    IsSuccess = false,
                    Message = "Cab not found.",
                    Data = null
                });

            return Ok(new ApiResponse<object>
            {
                IsSuccess = true,
                Message = "Cab retrieved successfully.",
                Data = cabs
            });
        }

        // POST: api/Portfolio
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Create([FromForm] CreateCabRequest request, CancellationToken cancellationToken = default)
        {
            var id = await _cabService.CreateAsync(request, cancellationToken);

            return CreatedAtAction(nameof(GetById), new { id }, new ApiResponse<object>
            {
                IsSuccess = true,
                Message = "Cab created successfully.",
                Data = id
            });
        }

        // PUT: api/Portfolios/{id}
        [HttpPut("{id:guid}")]
        [Authorize]
        public async Task<IActionResult> Update(Guid id, [FromForm] UpdateCabRequest request, CancellationToken cancellationToken = default)
        {
            request.Id = id;

            await _cabService.UpdateAsync(request, cancellationToken);

            return Ok(new ApiResponse<object>
            {
                IsSuccess = true,
                Message = "Cab updated successfully.",
                Data = id
            });
        }

        // DELETE: api/Cab/{id}
        [HttpDelete("{id:guid}")]
        [Authorize]
        public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken = default)
        {
            await _cabService.DeleteAsync(id, cancellationToken);

            return Ok(new ApiResponse<object>
            {
                IsSuccess = true,
                Message = "Cab deleted successfully.",
                Data = id
            });
        }
    }
}
