using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TravoRides.Application.DTOs.SelfDrive;
using TravoRides.Application.DTOs.Common;
using TravoRides.Application.Interfaces;
using TravoRiders.Application.Common.Responses;

namespace TravoRides.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SelfDriveController : ControllerBase
    {
        private readonly ISelfDriveService _service;
        public SelfDriveController(ISelfDriveService service) => _service = service;

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] SearchSelfDriveRequest request, CancellationToken cancellationToken = default)
        {
            var result = await _service.GetAllAsync(request, cancellationToken);
            return Ok(new ApiResponse<PagedResponse<SelfDriveDTO>> { IsSuccess = true, Message = "Items retrieved.", Data = result });
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> Get(Guid id, CancellationToken cancellationToken)
        {
            var r = await _service.GetByIdAsync(id, cancellationToken);
            if (r == null) return NotFound(new ApiResponse<object> { IsSuccess = false, Message = "Item not found." });
            return Ok(new ApiResponse<object> { IsSuccess = true, Message = "Item retrieved.", Data = r });
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Create([FromBody] CreateSelfDriveRequest request, CancellationToken cancellationToken)
        {
            var id = await _service.CreateAsync(request, cancellationToken);
            return CreatedAtAction(nameof(Get), new { id }, new ApiResponse<object> { IsSuccess = true, Message = "Created.", Data = id });
        }

        [HttpPut("{id:guid}")]
        [Authorize]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateSelfDriveRequest request, CancellationToken cancellationToken)
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
