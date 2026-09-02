using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TravoRides.Application.DTOs.Common;
using TravoRides.Application.Interfaces;
using TravoRiders.Application.Common.Responses;
using TravoRides.Application.DTOs.Package;

namespace TravoRides.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PackageController : ControllerBase
    {
        private readonly IPackageService _service;
        public PackageController(IPackageService service) => _service = service;

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] SearchPackageRequest request, CancellationToken cancellationToken = default)
        {
            var result = await _service.GetAllAsync(request, cancellationToken);
            return Ok(new ApiResponse<PagedResponse<PackageDTO>> { IsSuccess = true, Message = "Packages retrieved.", Data = result });
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> Get(Guid id, CancellationToken cancellationToken)
        {
            var r = await _service.GetByIdAsync(id, cancellationToken);
            if (r == null) return NotFound(new ApiResponse<object> { IsSuccess = false, Message = "Package not found." });
            return Ok(new ApiResponse<object> { IsSuccess = true, Message = "Package retrieved.", Data = r });
        }

        [HttpPost]
        //[Authorize]
        public async Task<IActionResult> Create([FromBody] CreatePackageRequest request, CancellationToken cancellationToken)
        {
            var id = await _service.CreateAsync(request, cancellationToken);
            return CreatedAtAction(nameof(Get), new { id }, new ApiResponse<object> { IsSuccess = true, Message = "Package created.", Data = id });
        }

        [HttpPut("{id:guid}")]
        //[Authorize]
        public async Task<IActionResult> Update(Guid id, [FromBody] PackageDTO request, CancellationToken cancellationToken)
        {
            request.Id = id;
            await _service.UpdateAsync(request, cancellationToken);
            return Ok(new ApiResponse<object> { IsSuccess = true, Message = "Package updated.", Data = id });
        }

        [HttpDelete("{id:guid}")]
        //[Authorize]
        public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
        {
            await _service.DeleteAsync(id, cancellationToken);
            return Ok(new ApiResponse<object> { IsSuccess = true, Message = "Package deleted.", Data = id });
        }
    }
}
