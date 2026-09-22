using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using TravoRides.Application.Common.Responses;
using TravoRides.Application.DTOs.Authentication;
using TravoRides.Application.DTOs.Common;
using TravoRides.CMS.Interface;
using TravoRides.CMS.Models;

namespace TravoRides.CMS.Services
{
    public class ApiService : IApiService
    {
        private readonly HttpClient _httpClient;
        private readonly IHttpContextAccessor _httpContextAccessor;

        private readonly ILogger<ApiService> _logger;

        public ApiService(
            HttpClient httpClient,
            IHttpContextAccessor httpContextAccessor, ILogger<ApiService> logger)
        {
            _httpClient = httpClient;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
        }

        // ============================================================
        // LOGIN
        // POST: api/Auth/login
        // ============================================================

        public async Task<ApiResponse<LoginResponse?>> LoginAsync(LoginModel model)
        {
            var response = await _httpClient.PostAsJsonAsync(
                "api/Auth/login",
                model);

            if (!response.IsSuccessStatusCode)
                return null;

            return await response.Content.ReadFromJsonAsync<ApiResponse<LoginResponse>>();
        }

        // ============================================================
        // REFRESH TOKEN
        // POST: api/Auth/refresh-token
        // ============================================================

        // ============================================================
        // REFRESH TOKEN
        // POST: api/Auth/refresh-token
        // Reads refresh_token from the auth cookie, calls the API, then
        // re-issues the cookie with the new tokens (preserving Remember Me).
        // ============================================================
        public async Task<LoginResponse?> RefreshTokenAsync()
        {
            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext == null)
                return null;

            var refreshToken = await httpContext.GetTokenAsync(
                CookieAuthenticationDefaults.AuthenticationScheme, "refresh_token");

            if (string.IsNullOrWhiteSpace(refreshToken))
                return null;

            var request = new RefreshTokenRequest { RefreshToken = refreshToken };

            // Refresh endpoint does not normally need the expired access token.
            var response = await _httpClient.PostAsJsonAsync("api/Auth/refresh-token", request);

            if (!response.IsSuccessStatusCode)
            {
                // Refresh token itself is invalid/expired — force logout
                await httpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                return null;
            }

            var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<LoginResponse>>();

            if (apiResponse == null || !apiResponse.IsSuccess || apiResponse.Data == null)
            {
                await httpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                return null;
            }

            var loginResponse = apiResponse.Data;

            // Re-issue the auth cookie with the new tokens, preserving the
            // original principal and IsPersistent/ExpiresUtc (Remember Me) settings
            var authenticateResult = await httpContext.AuthenticateAsync(
                CookieAuthenticationDefaults.AuthenticationScheme);

            if (!authenticateResult.Succeeded || authenticateResult.Principal == null)
                return null;

            var authProperties = authenticateResult.Properties!;
            authProperties.StoreTokens(new[]
            {
            new AuthenticationToken { Name = "access_token", Value = loginResponse.AccessToken },
            new AuthenticationToken { Name = "refresh_token", Value = loginResponse.RefreshToken },
            new AuthenticationToken
{
    Name = "expires_at",
    Value = DateTime.SpecifyKind(loginResponse.AccessTokenExpiresAt, DateTimeKind.Utc).ToString("O")
}
           // new AuthenticationToken { Name = "expires_at", Value = loginResponse.AccessTokenExpiresAt.ToString("O") }
        });

            await httpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                authenticateResult.Principal,
                authProperties);

            return loginResponse;
        }

        // ============================================================
        // LOGOUT
        // POST: api/Auth/logout
        // ============================================================
        public async Task<bool> LogoutAsync()
        {
            var httpContext = _httpContextAccessor.HttpContext;

            var refreshToken = httpContext == null
                ? null
                : await httpContext.GetTokenAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme, "refresh_token");

            if (string.IsNullOrWhiteSpace(refreshToken))
            {
                if (httpContext != null)
                    await httpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                return true;
            }

            var request = new RefreshTokenRequest { RefreshToken = refreshToken };
            var response = await _httpClient.PostAsJsonAsync("api/Auth/logout", request);

            if (httpContext != null)
                await httpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            return response.IsSuccessStatusCode;
        }


    

        // ============================================================
        // GET ALL
        // ============================================================

        public async Task<T> GetAllAsync<T>(string url)
        {
             await AddAuthorizationHeader();

            var response = await _httpClient.GetAsync(url);

            response.EnsureSuccessStatusCode();

            return await response.Content
                .ReadFromJsonAsync<T>();
        }

       
        // ============================================================
        // GET BY ID
        // ============================================================

        public async Task<T> GetAsync<T>(string url)
        {
            await AddAuthorizationHeader();

            var response = await _httpClient.GetAsync(url);

            response.EnsureSuccessStatusCode();

            return await response.Content
                .ReadFromJsonAsync<T>();
        }


        // ============================================================
        // POST WITH MODEL
        // ============================================================

        public async Task<TResponse> PostAsync<TRequest,TResponse>(string url,TRequest obj)
        {
            await AddAuthorizationHeader();

            var response = await _httpClient
                .PostAsJsonAsync(url, obj);

            response.EnsureSuccessStatusCode();

            return await response.Content
                .ReadFromJsonAsync<TResponse>();
        }


        // ============================================================
        // POST WITH HTTPCONTENT
        // ============================================================

        public async Task<T> PostAsync<T>(
            string url,
            HttpContent content)
        {
            await AddAuthorizationHeader();

            var response = await _httpClient
                .PostAsync(url, content);

            if (!response.IsSuccessStatusCode)
            {
                var errorText =
                    await response.Content.ReadAsStringAsync();

                throw new HttpRequestException(
                    $"Request to {url} failed " +
                    $"({(int)response.StatusCode} " +
                    $"{response.ReasonPhrase}): {errorText}");
            }

            return await response.Content
                .ReadFromJsonAsync<T>();
        }


        // ============================================================
        // PUT WITH MODEL
        // ============================================================
        public async Task<T> PutAsync<T>(
           string url, T obj)
        {
            await AddAuthorizationHeader();

            var response = await _httpClient
                .PutAsJsonAsync(url, obj);

            response.EnsureSuccessStatusCode();

            return await response.Content
                .ReadFromJsonAsync<T>();
        }
        public async Task<TResponse> PutAsync<TRequest,TResponse>(
            string url,TRequest obj)
        {
           await  AddAuthorizationHeader();

            var response = await _httpClient
                .PutAsJsonAsync(url, obj);

            response.EnsureSuccessStatusCode();

            return await response.Content
                .ReadFromJsonAsync<TResponse>();
        }


        // ============================================================
        // PUT WITH HTTPCONTENT
        // ============================================================

        public async Task<T> PutAsync<T>(
            string url,
            HttpContent content)
        {
            await AddAuthorizationHeader();

            var response = await _httpClient
                .PutAsync(url, content);

            response.EnsureSuccessStatusCode();

            return await response.Content
                .ReadFromJsonAsync<T>();
        }


        // ============================================================
        // DELETE
        // ============================================================

        public async Task<bool> DeleteAsync(string url)
        {
             await AddAuthorizationHeader();

            var response = await _httpClient
                .DeleteAsync(url);

            return response.IsSuccessStatusCode;
        }

        // <summary>
        // Helper method to add Bearer token from session to HttpCab default headers
        // </summary>
        //private void AddAuthorizationHeader()
        //{
        //    var session = _httpContextAccessor.HttpContext?.Session;

        //    var accessToken = session?.GetString("AccessToken");

        //    _httpClient.DefaultRequestHeaders.Remove("Authorization");

        //    if (string.IsNullOrWhiteSpace(accessToken))
        //    {
        //        // Remove auth header if no token available
        //        _httpClient.DefaultRequestHeaders.Authorization = null;
        //        return;
        //    }

        //    // Add Bearer token to Authorization header
        //    _httpClient.DefaultRequestHeaders.Authorization =
        //        new AuthenticationHeaderValue("Bearer", accessToken);

        //    var handler = new JwtSecurityTokenHandler();
        //    var jwt = handler.ReadJwtToken(accessToken);

        //}
        // ============================================================
        // AUTHORIZATION HEADER
        // Call this at the top of every other API method (GetAsync, PostAsync, etc.)
        // before making the request. Proactively refreshes the access token if
        // it's within 60 seconds of expiring.
        // ============================================================
        private async Task AddAuthorizationHeader()
        {
            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext == null)
                return;

            var accessToken = await httpContext.GetTokenAsync(
                CookieAuthenticationDefaults.AuthenticationScheme, "access_token");
            var expiresAtStr = await httpContext.GetTokenAsync(
                CookieAuthenticationDefaults.AuthenticationScheme, "expires_at");

            _httpClient.DefaultRequestHeaders.Authorization = null;

            if (string.IsNullOrWhiteSpace(accessToken))
                return;

            // Refresh a little early (60s buffer) so a request never lands
            // right as the token expires
            if (DateTimeOffset.TryParse(expiresAtStr, out var expiresAt) &&
                expiresAt <= DateTimeOffset.UtcNow.AddSeconds(60))
            {
                var refreshed = await RefreshTokenAsync();

                if (refreshed == null)
                    return; // RefreshTokenAsync already signed the user out on failure

                accessToken = refreshed.AccessToken;
            }

            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", accessToken);
        }

       
    }
}
