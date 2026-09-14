using TravoRides.Application.Common.Responses;
using TravoRides.CMS.Interface;
using TravoRides.CMS.Models;
using Microsoft.AspNetCore.Mvc;

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
    }
}
