using Microsoft.AspNetCore.Mvc;
using TravoRiders.Application.Common.Responses;
using TravoRides.Application.DTOs.BookingDTO;
using TravoRides.Application.DTOs.Category;
using TravoRides.Application.DTOs.Common;
using TravoRides.Application.Interfaces;

namespace TravoRides.API.Controllers
{
    public class BookingController : Controller
    {
        private readonly IBookingService _service;

        public BookingController(IBookingService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] SearchBookingRequest request, CancellationToken cancellationToken)
        {
            var result = await _service.GetAllAsync(request, cancellationToken);
            return Ok(new ApiResponse<PagedResponse<BookingDTO>> { IsSuccess = true, Message = "Bookings retrieved.", Data = result });
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> Get(Guid id, CancellationToken cancellationToken)
        {
            var r = await _service.GetByIdAsync(id, cancellationToken);
            if (r == null) return NotFound(new ApiResponse<object> { IsSuccess = false, Message = "Booking not found." });
            return Ok(new ApiResponse<object> { IsSuccess = true, Message = "Booking retrieved.", Data = r });
        }

        [HttpPost]
        //[Authorize]
        public async Task<IActionResult> Create([FromBody] CreateBookingRequest request, CancellationToken cancellationToken)
        {
            var id = await _service.CreateAsync(request, cancellationToken);
            return CreatedAtAction(nameof(Get), new { id }, new ApiResponse<object> { IsSuccess = true, Message = "Booking created.", Data = id });
        }

        [HttpPut("{id:guid}")]
        // [Authorize]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateBookingRequest request, CancellationToken cancellationToken)
        {
            request.Id = id;
            await _service.UpdateAsync(request, cancellationToken);
            return Ok(new ApiResponse<object> { IsSuccess = true, Message = "Booking updated.", Data = id });
        }

        [HttpDelete("{id:guid}")]
        //[Authorize]
        public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
        {
            await _service.DeleteAsync(id, cancellationToken);
            return Ok(new ApiResponse<object> { IsSuccess = true, Message = "Booking deleted.", Data = id });
        }
    }
}
