using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TravoRides.Application.DTOs.FeaturesMaster;
using TravoRides.Application.DTOs.Common;
using TravoRides.Application.Interfaces;
using TravoRiders.Application.Common.Responses;

namespace TravoRides.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FeaturesMasterController : ControllerBase
    {
        private readonly IFeaturesMasterService _service;
        public FeaturesMasterController(IFeaturesMasterService service) => _service = service;

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] SearchFeatureMasterRequest request, CancellationToken cancellationToken = default)
        {
            var result = await _service.GetAllAsync(request, cancellationToken);
            return Ok(new ApiResponse<PagedResponse<FeaturesMasterDTO>> { IsSuccess = true, Message = "Features retrieved.", Data = result });
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> Get(Guid id, CancellationToken cancellationToken)
        {
            var r = await _service.GetByIdAsync(id, cancellationToken);
            if (r == null) return NotFound(new ApiResponse<object> { IsSuccess = false, Message = "Feature not found." });
            return Ok(new ApiResponse<object> { IsSuccess = true, Message = "Feature retrieved.", Data = r });
        }

        [HttpPost]
        //[Authorize]
        public async Task<IActionResult> Create([FromBody] CreateFeaturesMasterRequest request, CancellationToken cancellationToken)
        {
            var id = await _service.CreateAsync(request, cancellationToken);
            return CreatedAtAction(nameof(Get), new { id }, new ApiResponse<object> { IsSuccess = true, Message = "Feature created.", Data = id });
        }

        [HttpPut("{id:guid}")]
        //[Authorize]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateFeaturesMasterRequest request, CancellationToken cancellationToken)
        {
            request.Id = id;
            await _service.UpdateAsync(request, cancellationToken);
            return Ok(new ApiResponse<object> { IsSuccess = true, Message = "Feature updated.", Data = id });
        }

        [HttpDelete("{id:guid}")]
        //[Authorize]
        public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
        {
            await _service.DeleteAsync(id, cancellationToken);
            return Ok(new ApiResponse<object> { IsSuccess = true, Message = "Feature deleted.", Data = id });
        }
    }
}
