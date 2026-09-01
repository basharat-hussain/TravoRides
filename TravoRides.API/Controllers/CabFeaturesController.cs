using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TravoRiders.Application.Common.Responses;
using TravoRides.Application.DTOs.CabFeatures;
using TravoRides.Application.DTOs.Common;
using TravoRides.Application.Interfaces;

namespace TravoRides.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CabFeaturesController : ControllerBase
    {
        private readonly ICabFeaturesService _service;
        public CabFeaturesController(ICabFeaturesService service) => _service = service;

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10, CancellationToken cancellationToken = default)
        {
            var result = await _service.GetAllAsync(pageNumber, pageSize, null, cancellationToken);
            return Ok(new ApiResponse<PagedResponse<CabFeaturesDTO>> { IsSuccess = true, Message = "Items retrieved.", Data = result });
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> Get(Guid id, CancellationToken cancellationToken)
        {
            var r = await _service.GetByIdAsync(id, cancellationToken);
            if (r == null) return NotFound(new ApiResponse<object> { IsSuccess = false, Message = "Not found." });
            return Ok(new ApiResponse<object> { IsSuccess = true, Message = "Retrieved.", Data = r });
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Create([FromBody] CreateCabFeaturesRequest request, CancellationToken cancellationToken)
        {
            var id = await _service.CreateAsync(request, cancellationToken);
            return CreatedAtAction(nameof(Get), new { id }, new ApiResponse<object> { IsSuccess = true, Message = "Created.", Data = id });
        }

        [HttpPut("{id:guid}")]
        [Authorize]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateCabFeaturesRequest request, CancellationToken cancellationToken)
        {
            request.Id = id;
            await _service.UpdateAsync(request, cancellationToken);
            return Ok(new ApiResponse<object> { IsSuccess = true, Message = "Updated.", Data = id });
        }

        [HttpDelete("{id:guid}")]
        [Authorize]
        public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
        {
            await _service.DeleteAsync(id, cancellationToken);
            return Ok(new ApiResponse<object> { IsSuccess = true, Message = "Deleted.", Data = id });
        }
    }
}
