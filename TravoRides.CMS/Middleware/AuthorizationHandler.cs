using System.Net.Http.Headers;

namespace TravoRides.CMS.Middleware
{
    public class AuthorizationHandler : DelegatingHandler
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AuthorizationHandler(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        //protected override async Task<HttpResponseMessage> SendAsync(
        //    HttpRequestMessage request,
        //    CancellationToken cancellationToken)
        //{
        //    var session = _httpContextAccessor.HttpContext?.Session;

        //    var accessToken = session?.GetString("AccessToken");

        //    if (!string.IsNullOrWhiteSpace(accessToken))
        //    {
        //        request.Headers.TryAddWithoutValidation("Authorization", accessToken);
        //    }

        //    return await base.SendAsync(request, cancellationToken);
        //}
    }
}
