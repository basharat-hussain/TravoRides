using TravoRides.Application.Common.Responses;
using TravoRides.Application.DTOs.Authentication;
using TravoRides.Application.DTOs.Common;
using TravoRides.CMS.Interface;
using TravoRides.CMS.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;

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

        public async Task<LoginResponse?> RefreshTokenAsync()
        {
            var session = _httpContextAccessor
                .HttpContext?
                .Session;

            var refreshToken = session?
                .GetString("RefreshToken");

            if (string.IsNullOrWhiteSpace(refreshToken))
                return null;

            var request = new RefreshTokenRequest
            {
                RefreshToken = refreshToken
            };

            // Refresh endpoint does not normally need
            // the expired access token.
            var response = await _httpClient.PostAsJsonAsync(
                "api/Auth/refresh-token",
                request);

            if (!response.IsSuccessStatusCode)
                return null;

            var apiResponse =
                await response.Content
                    .ReadFromJsonAsync<ApiResponse<LoginResponse>>();

            if (apiResponse == null ||
                !apiResponse.IsSuccess ||
                apiResponse.Data == null)
            {
                return null;
            }

            var loginResponse = apiResponse.Data;

            // Replace old tokens
            session?.SetString(
                "AccessToken",
                loginResponse.AccessToken);

            session?.SetString(
                "RefreshToken",
                loginResponse.RefreshToken);

            session?.SetString(
                "AccessTokenExpiresAt",
                loginResponse.AccessTokenExpiresAt.ToString("O"));

            session?.SetString(
                "RefreshTokenExpiresAt",
                loginResponse.RefreshTokenExpiresAt.ToString("O"));

            return loginResponse;
        }

        public async Task<bool> LogoutAsync()
        {
            var session = _httpContextAccessor.HttpContext?.Session;

            var refreshToken = session?.GetString("RefreshToken");

            if (string.IsNullOrWhiteSpace(refreshToken))
            {
                session?.Clear();
                return true;
            }

            var request = new RefreshTokenRequest
            {
                RefreshToken = refreshToken
            };

            var response = await _httpClient.PostAsJsonAsync(
                "api/Auth/logout",
                request
                );

            // Clear MVC session after logout
            session?.Clear();

            return response.IsSuccessStatusCode;
        }
        // ============================================================
        // GET ALL
        // ============================================================

        public async Task<T> GetAllAsync<T>(string url)
        {
          //  AddAuthorizationHeader();

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
            //AddAuthorizationHeader();

            var response = await _httpClient.GetAsync(url);

            response.EnsureSuccessStatusCode();

            return await response.Content
                .ReadFromJsonAsync<T>();
        }


        // ============================================================
        // POST WITH MODEL
        // ============================================================

        public async Task<T> PostAsync<T>(
            string url,object obj)
        {
           // AddAuthorizationHeader();

            var response = await _httpClient
                .PostAsJsonAsync(url, obj);

            response.EnsureSuccessStatusCode();

            return await response.Content
                .ReadFromJsonAsync<T>();
        }


        // ============================================================
        // POST WITH HTTPCONTENT
        // ============================================================

        public async Task<T> PostAsync<T>(
            string url,
            HttpContent content)
        {
            //AddAuthorizationHeader();

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
            string url,object obj)
        {
            //AddAuthorizationHeader();

            var response = await _httpClient
                .PutAsJsonAsync(url, obj);

            response.EnsureSuccessStatusCode();

            return await response.Content
                .ReadFromJsonAsync<T>();
        }


        // ============================================================
        // PUT WITH HTTPCONTENT
        // ============================================================

        public async Task<T> PutAsync<T>(
            string url,
            HttpContent content)
        {
            //AddAuthorizationHeader();

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
            //AddAuthorizationHeader();

            var response = await _httpClient
                .DeleteAsync(url);

            return response.IsSuccessStatusCode;
        }

        /// <summary>
        /// Helper method to add Bearer token from session to HttpCab default headers
        /// </summary>
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


    }
}