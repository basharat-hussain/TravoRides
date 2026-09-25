using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
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
        private static readonly SemaphoreSlim _refreshLock = new(1, 1);

        public ApiService(
            HttpClient httpClient,
            IHttpContextAccessor httpContextAccessor,
            ILogger<ApiService> logger)
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
            var response = await _httpClient.PostAsJsonAsync("api/Auth/login", model);

            if (!response.IsSuccessStatusCode)
                return null!;

            return (await response.Content.ReadFromJsonAsync<ApiResponse<LoginResponse>>())!;
        }

        // ============================================================
        // REFRESH TOKEN
        // POST: api/Auth/refresh-token
        // Reads refresh_token from the auth cookie/context, calls the API,
        // then re-issues the cookie with the new tokens (preserving Remember Me).
        // ============================================================
        public async Task<RefreshTokenResponse?> RefreshTokenAsync()
        {
            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext == null)
                return null;

            await _refreshLock.WaitAsync();
            try
            {
                // Double check if token was already refreshed while waiting for the lock
                var currentExpiresAtStr = httpContext.Items.TryGetValue("CurrentExpiresAt", out var inMemExp) && inMemExp is string expStr
                    ? expStr
                    : await httpContext.GetTokenAsync(CookieAuthenticationDefaults.AuthenticationScheme, "expires_at");

                if (!string.IsNullOrWhiteSpace(currentExpiresAtStr) &&
                    DateTimeOffset.TryParse(currentExpiresAtStr, CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal, out var inMemExpOffset) &&
                    inMemExpOffset > DateTimeOffset.UtcNow.AddSeconds(60))
                {
                    var currentAccessToken = httpContext.Items.TryGetValue("CurrentAccessToken", out var inMemToken) && inMemToken is string tok
                        ? tok
                        : await httpContext.GetTokenAsync(CookieAuthenticationDefaults.AuthenticationScheme, "access_token");

                    var currentRefreshToken = httpContext.Items.TryGetValue("CurrentRefreshToken", out var inMemRf) && inMemRf is string rf
                        ? rf
                        : await httpContext.GetTokenAsync(CookieAuthenticationDefaults.AuthenticationScheme, "refresh_token");

                    if (!string.IsNullOrWhiteSpace(currentAccessToken))
                    {
                        return new RefreshTokenResponse
                        {
                            AccessToken = currentAccessToken,
                            RefreshToken = currentRefreshToken ?? string.Empty,
                            AccessTokenExpiresAt = inMemExpOffset.UtcDateTime
                        };
                    }
                }

                var refreshToken = httpContext.Items.TryGetValue("CurrentRefreshToken", out var curRf) && curRf is string sRf && !string.IsNullOrWhiteSpace(sRf)
                    ? sRf
                    : await httpContext.GetTokenAsync(CookieAuthenticationDefaults.AuthenticationScheme, "refresh_token");

                if (string.IsNullOrWhiteSpace(refreshToken))
                {
                    _logger.LogWarning("RefreshTokenAsync called, but no refresh token was found.");
                    return null;
                }

                var request = new RefreshTokenRequest { RefreshToken = refreshToken };

                using var refreshMessage = new HttpRequestMessage(HttpMethod.Post, "api/Auth/refresh-token")
                {
                    Content = JsonContent.Create(request)
                };
                refreshMessage.Headers.Authorization = null;

                var response = await _httpClient.SendAsync(refreshMessage);

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning("Refresh token API call failed with status code: {StatusCode}", response.StatusCode);
                    if (!httpContext.Response.HasStarted)
                    {
                        await httpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                    }
                    return null;
                }

                var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<RefreshTokenResponse>>();

                if (apiResponse == null || !apiResponse.IsSuccess || apiResponse.Data == null)
                {
                    _logger.LogWarning("Refresh token API response was unsuccessful or empty.");
                    if (!httpContext.Response.HasStarted)
                    {
                        await httpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                    }
                    return null;
                }

                var refreshedData = apiResponse.Data;

                var authenticateResult = await httpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);

                if (!authenticateResult.Succeeded || authenticateResult.Principal == null)
                    return null;

                var authProperties = authenticateResult.Properties ?? new AuthenticationProperties();
                var formattedExpiresAt = DateTime.SpecifyKind(refreshedData.AccessTokenExpiresAt, DateTimeKind.Utc).ToString("O");

                authProperties.StoreTokens(new[]
                {
                    new AuthenticationToken { Name = "access_token", Value = refreshedData.AccessToken },
                    new AuthenticationToken { Name = "refresh_token", Value = refreshedData.RefreshToken },
                    new AuthenticationToken { Name = "expires_at", Value = formattedExpiresAt }
                });

                if (!httpContext.Response.HasStarted)
                {
                    await httpContext.SignInAsync(
                        CookieAuthenticationDefaults.AuthenticationScheme,
                        authenticateResult.Principal,
                        authProperties);
                }

                // Update in-memory items and feature so subsequent operations in this request see new tokens
                httpContext.Items["CurrentAccessToken"] = refreshedData.AccessToken;
                httpContext.Items["CurrentRefreshToken"] = refreshedData.RefreshToken;
                httpContext.Items["CurrentExpiresAt"] = formattedExpiresAt;

                var authFeature = httpContext.Features.Get<Microsoft.AspNetCore.Authentication.IAuthenticateResultFeature>();
                if (authFeature != null)
                {
                    authFeature.AuthenticateResult = AuthenticateResult.Success(
                        new AuthenticationTicket(authenticateResult.Principal, authProperties, CookieAuthenticationDefaults.AuthenticationScheme));
                }

                return refreshedData;
            }
            finally
            {
                _refreshLock.Release();
            }
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
                : (httpContext.Items.TryGetValue("CurrentRefreshToken", out var curRf) && curRf is string sRf && !string.IsNullOrWhiteSpace(sRf)
                    ? sRf
                    : await httpContext.GetTokenAsync(CookieAuthenticationDefaults.AuthenticationScheme, "refresh_token"));

            if (string.IsNullOrWhiteSpace(refreshToken))
            {
                if (httpContext != null && !httpContext.Response.HasStarted)
                    await httpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                return true;
            }

            var request = new RefreshTokenRequest { RefreshToken = refreshToken };
            using var logoutMessage = new HttpRequestMessage(HttpMethod.Post, "api/Auth/logout")
            {
                Content = JsonContent.Create(request)
            };
            logoutMessage.Headers.Authorization = null;

            var response = await _httpClient.SendAsync(logoutMessage);

            if (httpContext != null && !httpContext.Response.HasStarted)
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

            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                var refreshed = await RefreshTokenAsync();
                if (refreshed != null)
                {
                    _httpClient.DefaultRequestHeaders.Authorization =
                        new AuthenticationHeaderValue("Bearer", refreshed.AccessToken);
                    response = await _httpClient.GetAsync(url);
                }
            }

            response.EnsureSuccessStatusCode();

            return (await response.Content.ReadFromJsonAsync<T>())!;
        }

        // ============================================================
        // GET BY ID
        // ============================================================
        public async Task<T> GetAsync<T>(string url)
        {
            await AddAuthorizationHeader();

            var response = await _httpClient.GetAsync(url);

            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                var refreshed = await RefreshTokenAsync();
                if (refreshed != null)
                {
                    _httpClient.DefaultRequestHeaders.Authorization =
                        new AuthenticationHeaderValue("Bearer", refreshed.AccessToken);
                    response = await _httpClient.GetAsync(url);
                }
            }

            response.EnsureSuccessStatusCode();

            return (await response.Content.ReadFromJsonAsync<T>())!;
        }

        // ============================================================
        // POST WITH MODEL
        // ============================================================
        public async Task<TResponse> PostAsync<TRequest, TResponse>(string url, TRequest obj)
        {
            await AddAuthorizationHeader();

            var response = await _httpClient.PostAsJsonAsync(url, obj);

            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                var refreshed = await RefreshTokenAsync();
                if (refreshed != null)
                {
                    _httpClient.DefaultRequestHeaders.Authorization =
                        new AuthenticationHeaderValue("Bearer", refreshed.AccessToken);
                    response = await _httpClient.PostAsJsonAsync(url, obj);
                }
            }

            response.EnsureSuccessStatusCode();

            return (await response.Content.ReadFromJsonAsync<TResponse>())!;
        }

        // ============================================================
        // POST WITH HTTPCONTENT
        // ============================================================
        public async Task<T> PostAsync<T>(string url, HttpContent content)
        {
            await AddAuthorizationHeader();

            var response = await _httpClient.PostAsync(url, content);

            if (!response.IsSuccessStatusCode)
            {
                var errorText = await response.Content.ReadAsStringAsync();
                throw new HttpRequestException(
                    $"Request to {url} failed ({(int)response.StatusCode} {response.ReasonPhrase}): {errorText}");
            }

            return (await response.Content.ReadFromJsonAsync<T>())!;
        }

        // ============================================================
        // PUT WITH MODEL
        // ============================================================
        public async Task<T> PutAsync<T>(string url, T obj)
        {
            await AddAuthorizationHeader();

            var response = await _httpClient.PutAsJsonAsync(url, obj);

            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                var refreshed = await RefreshTokenAsync();
                if (refreshed != null)
                {
                    _httpClient.DefaultRequestHeaders.Authorization =
                        new AuthenticationHeaderValue("Bearer", refreshed.AccessToken);
                    response = await _httpClient.PutAsJsonAsync(url, obj);
                }
            }

            response.EnsureSuccessStatusCode();

            return (await response.Content.ReadFromJsonAsync<T>())!;
        }

        public async Task<TResponse> PutAsync<TRequest, TResponse>(string url, TRequest obj)
        {
            await AddAuthorizationHeader();

            var response = await _httpClient.PutAsJsonAsync(url, obj);

            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                var refreshed = await RefreshTokenAsync();
                if (refreshed != null)
                {
                    _httpClient.DefaultRequestHeaders.Authorization =
                        new AuthenticationHeaderValue("Bearer", refreshed.AccessToken);
                    response = await _httpClient.PutAsJsonAsync(url, obj);
                }
            }

            response.EnsureSuccessStatusCode();

            return (await response.Content.ReadFromJsonAsync<TResponse>())!;
        }

        // ============================================================
        // PUT WITH HTTPCONTENT
        // ============================================================
        public async Task<T> PutAsync<T>(string url, HttpContent content)
        {
            await AddAuthorizationHeader();

            var response = await _httpClient.PutAsync(url, content);

            response.EnsureSuccessStatusCode();

            return (await response.Content.ReadFromJsonAsync<T>())!;
        }

        // ============================================================
        // DELETE
        // ============================================================
        public async Task<bool> DeleteAsync(string url)
        {
            await AddAuthorizationHeader();

            var response = await _httpClient.DeleteAsync(url);

            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                var refreshed = await RefreshTokenAsync();
                if (refreshed != null)
                {
                    _httpClient.DefaultRequestHeaders.Authorization =
                        new AuthenticationHeaderValue("Bearer", refreshed.AccessToken);
                    response = await _httpClient.DeleteAsync(url);
                }
            }

            return response.IsSuccessStatusCode;
        }

        // ============================================================
        // AUTHORIZATION HEADER HELPER
        // Checks token expiration and proactively refreshes if within 60s
        // ============================================================
        private async Task<string?> GetValidAccessTokenAsync()
        {
            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext == null)
                return null;

            if (httpContext.Items.TryGetValue("CurrentAccessToken", out var currentTokenObj) &&
                currentTokenObj is string inMemoryToken && !string.IsNullOrWhiteSpace(inMemoryToken))
            {
                return inMemoryToken;
            }

            var accessToken = await httpContext.GetTokenAsync(
                CookieAuthenticationDefaults.AuthenticationScheme, "access_token");
            var expiresAtStr = await httpContext.GetTokenAsync(
                CookieAuthenticationDefaults.AuthenticationScheme, "expires_at");

            if (string.IsNullOrWhiteSpace(accessToken))
                return null;

            DateTimeOffset expiresAt = DateTimeOffset.MinValue;
            bool hasValidExpiry = false;

            if (!string.IsNullOrWhiteSpace(expiresAtStr) &&
                DateTimeOffset.TryParse(expiresAtStr, CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal, out var parsedExpiresAt))
            {
                expiresAt = parsedExpiresAt;
                hasValidExpiry = true;
            }
            else
            {
                try
                {
                    var handler = new JwtSecurityTokenHandler();
                    if (handler.CanReadToken(accessToken))
                    {
                        var jwt = handler.ReadJwtToken(accessToken);
                        expiresAt = new DateTimeOffset(jwt.ValidTo, TimeSpan.Zero);
                        hasValidExpiry = true;
                    }
                }
                catch
                {
                    // Ignore parse error
                }
            }

            if (hasValidExpiry && expiresAt <= DateTimeOffset.UtcNow.AddSeconds(60))
            {
                _logger.LogInformation("Access token is expiring or expired at {ExpiresAt}. Refreshing...", expiresAt);
                var refreshed = await RefreshTokenAsync();
                if (refreshed != null)
                {
                    accessToken = refreshed.AccessToken;
                }
                else
                {
                    return null;
                }
            }

            return accessToken;
        }

        private async Task AddAuthorizationHeader()
        {
            _httpClient.DefaultRequestHeaders.Authorization = null;

            var accessToken = await GetValidAccessTokenAsync();
            if (!string.IsNullOrWhiteSpace(accessToken))
            {
                _httpClient.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", accessToken);
            }
        }
    }
}

