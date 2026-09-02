using TravoRiders.Application.Common.Exceptions;
using TravoRides.Application.Interfaces;
using System.Security.Claims;

namespace TravoRides.API.Services
{
    public class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CurrentUserService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public bool IsAuthenticated => _httpContextAccessor.HttpContext?.User.Identity?.IsAuthenticated ?? false;

        public Guid UserId
        {
            get
            {
                var value = _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);

                if (!Guid.TryParse(value, out var userId))
                    throw new ResourceNotFoundException("User ID not found.");

                return userId;
            }
        }

        public string? Email => _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.Email);

        public string? Role => _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.Role);
    }
}
