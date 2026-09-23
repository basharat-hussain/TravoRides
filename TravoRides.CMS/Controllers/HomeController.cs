using Microsoft.AspNetCore.Mvc;
using TravoRides.Application.Common.Responses;
using TravoRides.Application.DTOs.Authentication;
using TravoRides.CMS.Interface;
using TravoRides.CMS.Models;

namespace TravoRides.CMS.Controllers
{
    public class HomeController : Controller
    {
        private readonly IApiService _apiService;

        public HomeController(IApiService apiService)
        {
            _apiService = apiService;
        }

        public async Task<IActionResult> Index()
        {      
            var response = await _apiService.GetAsync<ApiResponse<DashboardModel>>(
                "api/Dashboard");

            if (response == null || !response.IsSuccess)
            {
                return View(new DashboardModel());
            }

            return View(response.Data);
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
