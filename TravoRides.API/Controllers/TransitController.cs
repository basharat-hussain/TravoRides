using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TravoRides.Application.Common.Responses;
using TravoRides.Application.DTOs.Cabs;
using TravoRides.Application.DTOs.Common;
using TravoRides.Application.DTOs.Package;
using TravoRides.Application.DTOs.PackageRate;
using TravoRides.Application.DTOs.Transit;
using TravoRides.Application.DTOs.TransitRate;
using TravoRides.Application.Interfaces;

namespace TravoRides.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TransitController : ControllerBase
    {
        private readonly ITransitService _service;

        public TransitController(ITransitService service, IBookingService bookingService)
        {
            _service = service;
           
        }
        ///----------------==========  GET APIS -------------------------
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
       
        [HttpGet("{id:guid}/rates/{cabid:guid}")]
        public async Task<IActionResult> GetTransitRates(Guid id, Guid cabid, CancellationToken cancellationToken)
        {
            var rates = await _service.GetTransitRateAsync(cabid, id, cancellationToken);
            return Ok(new ApiResponse<TransitCabRateDTO>
            {
                IsSuccess = true,
                Data = rates,
                Message = "Transit rates fetched successfully",
            });
        }

        //========================== POST APIs ========================================
       
        [HttpPost]
        //[Authorize]
        public async Task<IActionResult> Create([FromForm] CreateTransitRequest request, CancellationToken cancellationToken)
        {
            var id = await _service.CreateAsync(request, cancellationToken);
            return CreatedAtAction(nameof(Get), new { id }, new ApiResponse<object> { IsSuccess = true, Message = "Created.", Data = id });
        }
       
        [HttpPost("{transitId:guid}/cabs")]
        public async Task<IActionResult> AddCabsToTransit( Guid transitId, [FromBody] TransitCabRequest request,  CancellationToken cancellationToken)
        {
            await _service.AddCabsToTransitAsync( transitId,  request, cancellationToken);

            return Ok(new ApiResponse<object>
            {
                IsSuccess = true,
                Message = "Cabs added to transit successfully."
            });
        }
        
        //=============================== PUT APIs =============================

        [HttpPut("{id:guid}")]
        // [Authorize]
        public async Task<IActionResult> Update(Guid id, [FromForm] UpdateTransitRequest request, CancellationToken cancellationToken)
        {
            request.Id = id;
            await _service.UpdateAsync(request, cancellationToken);
            return Ok(new ApiResponse<object> { IsSuccess = true, Message = "Updated.", Data = id });
        }

        [HttpPut("{packageId:guid}/cabs/{cabId:guid}")]
        public async Task<IActionResult> UpdateTransitCab( Guid packageId, Guid cabId,[FromBody] UpdateTransitCabRequest request, CancellationToken cancellationToken)
        {
            await _service.UpdateTransitCabAsync( packageId, cabId, request,  cancellationToken);

            return Ok(new ApiResponse<object>
            {
                IsSuccess = true,
                Message = "Transit cab rate updated successfully."
            });
        }

        //====================================== DELETE =============================
       
        [HttpDelete("{id:guid}")]
        //[Authorize]
        public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
        {
            await _service.DeleteAsync(id, cancellationToken);
            return Ok(new ApiResponse<object> { IsSuccess = true, Message = "Deleted.", Data = id });
        }

        [HttpDelete("{transitId:guid}/cabs/{cabId:guid}")]
        public async Task<IActionResult> RemoveCabFromTransit(Guid transitId, Guid cabId, CancellationToken cancellationToken)
        {
            await _service.RemoveCabFromTransitAsync(transitId, cabId, cancellationToken);

            return Ok(new ApiResponse<object>
            {
                IsSuccess = true,
                Message = "Cab removed from transit successfully."
            });
        }
      
    }
}
