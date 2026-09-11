using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TravoRides.Application.Common.Responses;
using TravoRides.Application.DTOs.Cabs;
using TravoRides.Application.DTOs.Common;
using TravoRides.Application.DTOs.Package;
using TravoRides.Application.DTOs.Transit;
using TravoRides.Application.Interfaces;

namespace TravoRides.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TransitController : ControllerBase
    {
        private readonly ITransitService _service;
        public TransitController(ITransitService service) => _service = service;

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] SearchTransitRequest request, CancellationToken cancellationToken = default)
        {
            var result = await _service.GetAllAsync(request, cancellationToken);
            return Ok(new ApiResponse<PagedResponse<TransitDTO>> { IsSuccess = true, Message = "Items retrieved.", Data = result });
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> Get(Guid id, CancellationToken cancellationToken)
        {
            var r = await _service.GetByIdAsync(id, cancellationToken);
            if (r == null) return NotFound(new ApiResponse<object> { IsSuccess = false, Message = "Not found." });
            return Ok(new ApiResponse<object> { IsSuccess = true, Message = "Retrieved.", Data = r });
        }

        [HttpGet("{transitId:guid}/cabs")]
        public async Task<IActionResult> GetCabsWithRates(Guid transitId, CancellationToken cancellationToken)
        {
            var result = await _service.GetCabsWithRatesAsync(transitId, cancellationToken);

            return Ok(new ApiResponse<List<TransitCabRateDTO>>
            {
                IsSuccess = true,
                Message = "Available cabs retrieved successfully.",
                Data = result
            });
        }
        [HttpPost]
        //[Authorize]
        public async Task<IActionResult> Create([FromForm] CreateTransitRequest request, CancellationToken cancellationToken)
        {
            var id = await _service.CreateAsync(request, cancellationToken);
            return CreatedAtAction(nameof(Get), new { id }, new ApiResponse<object> { IsSuccess = true, Message = "Created.", Data = id });
        }

        [HttpPut("{id:guid}")]
       // [Authorize]
        public async Task<IActionResult> Update(Guid id, [FromForm] UpdateTransitRequest request, CancellationToken cancellationToken)
        {
            request.Id = id;
            await _service.UpdateAsync(request, cancellationToken);
            return Ok(new ApiResponse<object> { IsSuccess = true, Message = "Updated.", Data = id });
        }

        [HttpDelete("{id:guid}")]
        //[Authorize]
        public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
        {
            await _service.DeleteAsync(id, cancellationToken);
            return Ok(new ApiResponse<object> { IsSuccess = true, Message = "Deleted.", Data = id });
        }
    }
}
