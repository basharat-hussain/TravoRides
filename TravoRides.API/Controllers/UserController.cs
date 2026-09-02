using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TravoRiders.Application.Common.Responses;
using TravoRiders.Application.DTOs.Users;
using TravoRides.Application.Interfaces;

namespace TravoRides.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ICurrentUserService _currentUserService;

        public UserController(IUserService userService, ICurrentUserService currentUserService)
        {
            _userService = userService;
            _currentUserService = currentUserService;
        }

        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> RegisterUser([FromBody] CreateUserRequst request)
        {
            var result = await _userService.RegisterUserAsync(request);
            return Ok(new ApiResponse<object>
            {
                IsSuccess = true,
                Message = "User registered successfully.",
                Data = result
            });
        }

        [HttpGet("me")]
        //[Authorize]
        public async Task<IActionResult> GetMyProfile()
        {
            var userId = _currentUserService.UserId;
            var userProfile = await _userService.GetMyProfileAsync(userId);
            return Ok(new ApiResponse<object>
            {
                IsSuccess = true,
                Message = "User details fetched successfully.",
                Data = userProfile
            });

        }

    }

}
