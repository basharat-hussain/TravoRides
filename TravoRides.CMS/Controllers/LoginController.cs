
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
                 new Claim(ClaimTypes.Name,           model.Email),
               // new Claim(ClaimTypes.NameIdentifier, result.Data.UserId ?? model.Email)
       
                 new Claim(ClaimTypes.NameIdentifier, model.Email)
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

            // 3. Carry the API token inside the encrypted ticket
            authProperties.StoreTokens(new[]
            {
        new AuthenticationToken { Name = "access_token", Value = result.Data.AccessToken }
    });

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                principal,
                authProperties);

            return Json(new[] { "True", "Login successfull" });
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await _apiService.LogoutAsync();

            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            HttpContext.Session.Clear(); // remove if you're no longer using Session anywhere

            return RedirectToAction("Index", "Login"); 
        }
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

            var response = await _apiService.PostAsync<ForgotPasswordRequest,ApiResponse<object>>("api/Auth/send-forgot-password-otp", model
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

            var response = await _apiService.PostAsync<VerifyPasswordResetOtpRequest,ApiResponse<object>>("api/Auth/verify-password-reset-otp", model
            );

            return Json(response);
        }

        [HttpPost]
        public async Task<IActionResult> ResetPassword( ResetPasswordRequest model)
        {
            if (!ModelState.IsValid)
            {
                return Json(new
                {
                    isSuccess = false,
                    message = "Please enter a valid password."
                });
            }

            var response = await _apiService.PostAsync<ResetPasswordRequest,ApiResponse<object>>("api/Auth/reset-password", model
            );

            return Json(response);
        }
    }
}

