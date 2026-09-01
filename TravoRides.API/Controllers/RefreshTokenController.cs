using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TravoRiders.Application.Common.Responses;
using TravoRides.Application.DTOs.RefreshTokens;
using TravoRides.Application.Interfaces;

namespace TravoRides.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RefreshTokenController : ControllerBase
    {
        private readonly IRefreshTokenService _service;
        public RefreshTokenController(IRefreshTokenService service) => _service = service;

        [HttpGet("by-token")]
        public async Task<IActionResult> GetByToken([FromQuery] string token, CancellationToken cancellationToken)
        {
            var r = await _service.GetByTokenAsync(token, cancellationToken);
            if (r == null) return NotFound(new ApiResponse<object> { IsSuccess = false, Message = "Token not found." });
            return Ok(new ApiResponse<object> { IsSuccess = true, Message = "Token retrieved.", Data = r });
        }

        [HttpGet("by-user/{userId:guid}")]
        public async Task<IActionResult> GetByUser(Guid userId, CancellationToken cancellationToken)
        {
            var r = await _service.GetByUserIdAsync(userId, cancellationToken);
            if (r == null) return NotFound(new ApiResponse<object> { IsSuccess = false, Message = "Not found." });
            return Ok(new ApiResponse<object> { IsSuccess = true, Message = "Retrieved.", Data = r });
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Create([FromBody] RefreshTokenDTO request, CancellationToken cancellationToken)
        {
            var id = await _service.CreateAsync(request, cancellationToken);
            return CreatedAtAction(nameof(GetByUser), new { userId = request.UserId }, new ApiResponse<object> { IsSuccess = true, Message = "Created.", Data = id });
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
