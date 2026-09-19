
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
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
        public async Task<IActionResult> Login(LoginModel model)
        {
            var response = new string[] { };

            if (!ModelState.IsValid)
            {
                response = new[] { "False", "Validation Failed" };
                return Json(response);
            }

            var result = await _apiService.LoginAsync(model);

            if (result == null || result.Data == null || string.IsNullOrWhiteSpace(result.Data.AccessToken))
            {
                response = new string[] { "False", "Invalid username or password." };
                return Json(response);
            }

            // Store token in Session
            HttpContext.Session.SetString("AccessToken", result.Data.AccessToken);

            response = new string[] { "True", "Login successful" };
            return Json(response);

            // Login successful → Dashboard
            //return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await _apiService.LogoutAsync();

            return RedirectToAction("Login", "Index");
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

