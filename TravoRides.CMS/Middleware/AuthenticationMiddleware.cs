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

            // -----------------------------------------
            // 2. Logged in user
            // -----------------------------------------
            if (isAuthenticated)
            {
                // Handle logout
                if (path.StartsWithSegments("/Login/Logout"))
                {
                    await apiService.LogoutAsync();
                    await context.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                    context.Response.Redirect("/Login/Index");
                    return;
                }

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
                            await context.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                            context.Response.Redirect("/Login/Index");
                            return;
                        }
                    }
                }

                await _next(context);
                return;
            }

            // -----------------------------------------
            // 3. Not logged in / anonymous access
            // -----------------------------------------
            if (path.StartsWithSegments("/Login"))
            {
                await _next(context);
                return;
            }

            // Everything else requires authentication
            context.Response.Redirect("/Login/Index");
        }
    }
}

