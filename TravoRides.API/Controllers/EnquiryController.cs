using TravoRiders.Application.Common.Responses;
using TravoRiders.Application.DTOs.Common;
using TravoRiders.Application.DTOs.Enquirer;
using TravoRiders.Application.DTOs.Review;
using TravoRiders.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace TravoRiders.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EnquiryController : ControllerBase
    {
        private readonly IEnquiryService _service;

        public EnquiryController(IEnquiryService service)
        {
            _service = service;
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetAll([FromQuery]SearchEnquiryRequest request, CancellationToken cancellationToken = default)
        {
            var enquiry = await _service.GetAllAsync(request, cancellationToken);

            return Ok(new ApiResponse<PagedResponse<EnquiryDTO>>
            {
                IsSuccess = true,
                Message = "Enquiry retrieved successfully.",
                Data = enquiry
            });
        }

        // GET: api/Enquiry/{id}
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken = default)
        {
            var enquiry = await _service.GetByIdAsync(id, cancellationToken);

            if (enquiry == null)
                return NotFound(new ApiResponse<object>
                {
                    IsSuccess = false,
                    Message = "Enquiry not found.",
                    Data = null
                });

            return Ok(new ApiResponse<object>
            {
                IsSuccess = true,
                Message = "Enquiry retrieved successfully.",
                Data = enquiry
            });
        }

        // POST: api/specializations
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateEnquiryRequest request, CancellationToken cancellationToken = default)
        {
            var id = await _service.CreateAsync(request, cancellationToken);

            return CreatedAtAction(nameof(GetById), new { id }, new ApiResponse<object>
            {
                IsSuccess = true,
                Message = "Enquiry Submitted successfully.",
                Data = id
            });
        }



        // DELETE: api/Enquiry/{id}
        [HttpDelete("{id:guid}")]
        [Authorize]
        public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken = default)
        {
            await _service.DeleteAsync(id, cancellationToken);

            return Ok(new ApiResponse<object>
            {
                IsSuccess = true,
                Message = "Enquiry deleted successfully.",
                Data = id
            });
        }
    }
}
