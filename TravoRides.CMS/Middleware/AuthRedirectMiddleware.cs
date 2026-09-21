namespace TravoRides.CMS.Middleware
{
    public class AuthRedirectMiddleware
    {
        private readonly RequestDelegate _next;
        public AuthRedirectMiddleware(RequestDelegate next) => _next = next;

        public async Task InvokeAsync(HttpContext context)
        {
            var path = context.Request.Path;
            var isAuthenticated = context.User.Identity?.IsAuthenticated == true;

            if (isAuthenticated && path.StartsWithSegments("/Login") &&
                !path.StartsWithSegments("/Login/Logout"))
            {
                context.Response.Redirect("/Home/Index");
                return;
            }

            await _next(context);
        }
    }
}
