using System.IdentityModel.Tokens.Jwt;

namespace TravoRides.CMS.Middleware
{
    public class AuthenticationMiddleware
    {
        private readonly RequestDelegate _next;

        public AuthenticationMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var path = context.Request.Path;

            var accessToken = context.Session.GetString("AccessToken");

            // Check whether token exists and is not expired
            var isTokenValid = IsTokenValid(accessToken);

            // -----------------------------------------
            // Logged in + valid token
            // -----------------------------------------
            if (isTokenValid)
            {
                // Don't allow logged-in users to go back to Login
                if (path.StartsWithSegments("/Login/Logout"))
                {
                    context.Session.Remove("AccessToken");
                    context.Response.Redirect("/Login");
                    return;

                }
                if (path.StartsWithSegments("/Login"))
                {
                    context.Response.Redirect("/Home/Index");
                    return;
                }

                await _next(context);
                return;
            }

            // -----------------------------------------
            // Not logged in OR token expired
            // -----------------------------------------

            if (!string.IsNullOrWhiteSpace(accessToken))
            {
                // Remove expired/invalid authentication
                context.Session.Remove("AccessToken");
            }

            // Login pages (and logout POST) must remain accessible when not authenticated
            if (path.StartsWithSegments("/Login") || path.StartsWithSegments("/Login/Logout"))
            {
                // Allow GET (show login page) and POST (login form submission or logout) to proceed
                // even when no token is present. Return after calling next to avoid redirecting
                // back to the login page.
                var method = context.Request.Method;
                if (string.Equals(method, "GET", StringComparison.OrdinalIgnoreCase) ||
                    string.Equals(method, "POST", StringComparison.OrdinalIgnoreCase))
                {
                    await _next(context);
                    return;
                }
            }

            // Static files
            if (path.StartsWithSegments("/css") ||
                path.StartsWithSegments("/js") ||
                path.StartsWithSegments("/images") ||
                path.StartsWithSegments("/favicon.ico"))
            {
                await _next(context);
                return;
            }

            // Everything else requires login
            context.Response.Redirect("/Login/Index");
        }

        private static bool IsTokenValid(string? token)
        {
            if (string.IsNullOrWhiteSpace(token))
                return false;

            try
            {
                var handler = new JwtSecurityTokenHandler();

                if (!handler.CanReadToken(token))
                    return false;

                var jwtToken = handler.ReadJwtToken(token);

                return jwtToken.ValidTo > DateTime.UtcNow;
            }
            catch
            {
                return false;
            }
        }
    }
}
