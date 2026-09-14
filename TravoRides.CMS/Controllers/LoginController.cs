using TravoRides.Application.Common.Responses;
using TravoRides.Application.DTOs.Authentication;
using TravoRides.CMS.Interface;
using TravoRides.CMS.Models;
using TravoRides.CMS.Services;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;

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
        public async Task<IActionResult> Logout(
       )
        {
            await _apiService.LogoutAsync();

            return RedirectToAction("Login", "Index");
        }
    }
}

