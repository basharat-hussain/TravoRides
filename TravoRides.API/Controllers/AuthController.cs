using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TravoRiders.Application.Common.Responses;
using TravoRiders.Application.DTOs.Authentication;
using TravoRiders.Application.Interfaces;
using TravoRiders.Application.Common.Responses;

namespace TravoRiders.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
           
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request, CancellationToken cancellationToken)
        {
            var response = await _authService.LoginAsync(request, cancellationToken);
            return Ok(new ApiResponse<object>
            {
                IsSuccess = true,
                Message = "Login successful",
                Data = response
            });
        }

        [HttpPost("refresh-token")]
        [Authorize]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest request, CancellationToken cancellationToken)
        {
            var result = await _authService.RefreshTokenAsync(request, cancellationToken);

            return Ok(new ApiResponse<object>
            {
                IsSuccess = true,
                Message = "Refresh Token Generated Successfully",
                Data = result
            });
        }

        [HttpPost("logout")]
        [Authorize]
        public async Task<IActionResult> Logout([FromBody] RefreshTokenRequest request, CancellationToken cancellationToken)
        {
            await _authService.LogoutAsync(request.RefreshToken, cancellationToken);

            return Ok(new ApiResponse<object>
            {
                IsSuccess = true,
                Message = "Logged out successfully."
            });
        }

       
    
    }
}
