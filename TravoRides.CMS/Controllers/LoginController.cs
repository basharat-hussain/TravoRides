
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Net.Http;
using System.Security.Claims;
using TravoRides.Application.Common.Responses;
using TravoRides.Application.DTOs.Authentication;
using TravoRides.CMS.Interface;
using TravoRides.CMS.Models;

namespace TravoRides.CMS.Controllers
{

    public class LoginController : Controller
    {
        private readonly IApiService _apiService;


        public LoginController(IApiService apiService)
        {
            _apiService = apiService;
        }

        [HttpGet]
        public IActionResult Index() => View();

        [HttpPost]
        [AllowAnonymous]
        //[ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<IActionResult> Login(LoginModel model)
        {
            if (!ModelState.IsValid)
                return Json(new[] { "False", "Validation Failed" });

            var result = await _apiService.LoginAsync(model);

            if (result?.Data == null || string.IsNullOrWhiteSpace(result.Data.AccessToken))
                return Json(new[] { "False", "Invalid email or password." });

            // 1. Build the identity
            var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, result.Data.UserId.ToString() ?? model.Email),
            new Claim(ClaimTypes.Email, result.Data.Email),
            new Claim(ClaimTypes.Role, result.Data.Role)
        };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            // 2. Remember Me logic
            var authProperties = new AuthenticationProperties
            {
                IsPersistent = model.RememberMe,
                ExpiresUtc = model.RememberMe
                    ? DateTimeOffset.UtcNow.AddDays(30)
                    : DateTimeOffset.UtcNow.AddHours(1),
                AllowRefresh = true
            };

            // 3. Carry the API tokens inside the encrypted cookie —
            // access token, refresh token, and absolute expiry of the access token
            authProperties.StoreTokens(new[]
            {
            new AuthenticationToken { Name = "access_token", Value = result.Data.AccessToken },
            new AuthenticationToken { Name = "refresh_token", Value = result.Data.RefreshToken },
            new AuthenticationToken { Name = "expires_at", Value = result.Data.AccessTokenExpiresAt.ToString("O") }
        });

            await HttpContext.SignInAsync( CookieAuthenticationDefaults.AuthenticationScheme, principal, authProperties);

            return Json(new[] { "True", "Login successful" });
        }

        public async Task<IActionResult> Logout()
        {
            await _apiService.LogoutAsync();
            // ApiService.LogoutAsync already calls SignOutAsync internally,
            // but this is a safe no-op if it was already cleared.
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Login");
        }

        public IActionResult AccessDenied() => View();

        [AllowAnonymous]
        [HttpGet]

        public IActionResult ForgotPassword()
        {
            return View();
        }

        // [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> SendForgotPasswordOtp(ForgotPasswordRequest model)
        {
            if (!ModelState.IsValid)
            {
                return Json(new
                {
                    isSuccess = false,
                    message = "Please enter a valid email address."
                });
            }

            var response = await _apiService.PostAsync<ForgotPasswordRequest, ApiResponse<object>>("api/Auth/send-forgot-password-otp", model
            );

            return Json(response);
        }

        [HttpPost]
        public async Task<IActionResult> VerifyPasswordResetOtp(VerifyPasswordResetOtpRequest model)
        {
            if (!ModelState.IsValid)
            {
                return Json(new
                {
                    isSuccess = false,
                    message = "Invalid OTP."
                });
            }

            var response = await _apiService.PostAsync<VerifyPasswordResetOtpRequest, ApiResponse<object>>("api/Auth/verify-password-reset-otp", model
            );

            return Json(response);
        }

        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordRequest model)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState
            .Where(x => x.Value.Errors.Any())
            .Select(x => new
            {
                Field = x.Key,
                Errors = x.Value.Errors.Select(e => e.ErrorMessage)
            })
            .ToList();
                return Json(new
                {
                    isSuccess = false,
                    message = "Please enter a valid password."
                });
            }

            var response = await _apiService.PostAsync<ResetPasswordRequest, ApiResponse<object>>("api/Auth/reset-password", model
            );

            return Json(response);
        }


        [HttpGet]
        public IActionResult ChangePassword() => View();

        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangePasswordRequest model)
        {
            if (!ModelState.IsValid)
            {
                return Json(new
                {
                    isSuccess = false,
                    message = "Please enter valid password details."
                });
            }

            var response = await _apiService.PostAsync<ChangePasswordRequest, ApiResponse<object>>(
                "api/Auth/change-password",
                model
            );

            return Json(response);
        }
    }
}

