using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using TravoRides.CMS.Interface;

namespace TravoRides.CMS.Middleware
{
    public class AuthenticationMiddleware
    {
        private readonly RequestDelegate _next;

        public AuthenticationMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, IApiService apiService)
        {
            var path = context.Request.Path;

            // -----------------------------------------
            // 1. Static files bypass auth logic
            // -----------------------------------------
            if (path.StartsWithSegments("/css") ||
                path.StartsWithSegments("/js") ||
                path.StartsWithSegments("/images") ||
                path.StartsWithSegments("/lib") ||
                path.StartsWithSegments("/assets") ||
                path.StartsWithSegments("/favicon.ico"))
            {
                await _next(context);
                return;
            }

            var isAuthenticated = context.User.Identity?.IsAuthenticated == true;

            // Handle logout
            if (path.StartsWithSegments("/Login/Logout"))
            {
                await apiService.LogoutAsync();
                context.Response.Cookies.Delete("MyApp.RefreshToken");
                await context.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                context.Response.Redirect("/Login/Index");
                return;
            }

            // -----------------------------------------
            // 2. Logged in user
            // -----------------------------------------
            if (isAuthenticated)
            {
                // Redirect away from login pages if already logged in
                if (path.StartsWithSegments("/Login"))
                {
                    context.Response.Redirect("/Home/Index");
                    return;
                }

                // -----------------------------------------
                // Token Expiry Check & Refresh Token Setup
                // -----------------------------------------
                var accessToken = await context.GetTokenAsync(CookieAuthenticationDefaults.AuthenticationScheme, "access_token");
                var expiresAtStr = await context.GetTokenAsync(CookieAuthenticationDefaults.AuthenticationScheme, "expires_at");

                if (!string.IsNullOrWhiteSpace(accessToken))
                {
                    bool isExpiringOrExpired = false;

                    if (!string.IsNullOrWhiteSpace(expiresAtStr) &&
                        DateTimeOffset.TryParse(expiresAtStr, CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal, out var expiresAt))
                    {
                        isExpiringOrExpired = expiresAt <= DateTimeOffset.UtcNow.AddSeconds(60);
                    }
                    else
                    {
                        try
                        {
                            var handler = new JwtSecurityTokenHandler();
                            if (handler.CanReadToken(accessToken))
                            {
                                var jwt = handler.ReadJwtToken(accessToken);
                                isExpiringOrExpired = jwt.ValidTo <= DateTime.UtcNow.AddSeconds(60);
                            }
                        }
                        catch
                        {
                            isExpiringOrExpired = true;
                        }
                    }

                    if (isExpiringOrExpired)
                    {
                        var refreshed = await apiService.RefreshTokenAsync();
                        if (refreshed == null)
                        {
                            // Refresh token itself is expired or revoked: sign out and redirect to login
                            context.Response.Cookies.Delete("MyApp.RefreshToken");
                            await context.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

                            if (context.Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                            {
                                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                                return;
                            }

                            context.Response.Redirect("/Login/Index");
                            return;
                        }
                    }
                }

                await _next(context);
                return;
            }

            // -----------------------------------------
            // 3. User session expired or unauthenticated
            // Attempt to refresh token and assign a new session!
            // -----------------------------------------
            if (context.Request.Cookies.ContainsKey("MyApp.RefreshToken"))
            {
                var refreshed = await apiService.RefreshTokenAsync();
                if (refreshed != null)
                {
                    // Successfully refreshed token and assigned new session
                    if (path.StartsWithSegments("/Login"))
                    {
                        context.Response.Redirect("/Home/Index");
                        return;
                    }

                    await _next(context);
                    return;
                }
            }

            // -----------------------------------------
            // 4. Truly anonymous access / Login page
            // -----------------------------------------
            if (path.StartsWithSegments("/Login"))
            {
                await _next(context);
                return;
            }

            // For AJAX requests where authentication/refresh failed, return 401 instead of redirecting HTML
            if (context.Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                return;
            }

            // Everything else requires authentication
            context.Response.Redirect("/Login/Index");
        }
    }
}

