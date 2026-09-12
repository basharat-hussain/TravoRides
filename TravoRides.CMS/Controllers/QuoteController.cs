using AlArwaSolutions.Application.Common.Responses;
using AlArwaSolutions.Application.DTOs.Common;
using AlArwaSolutions.Application.DTOs.Quote;
using AlArwaSolutions.Application.DTOs.Review;
using AlArwaSolutions.CMS.Interface;
using Microsoft.AspNetCore.Mvc;

namespace AlArwaSolutions.CMS.Controllers
{
    public class QuoteController : Controller
    {
        private readonly IApiService _apiService;

        public QuoteController(IApiService apiService)
        {
            _apiService = apiService;
        }

        [HttpGet]
        public async Task<IActionResult> Index(int? pageNumber, int? pageSize)
        {
            var page = pageNumber ?? 1;
            var size = pageSize ?? 10;

            var url = $"api/Quote?pageNumber={page}&pageSize={size}";

            var items = await _apiService.GetAllAsync<ApiResponse<PagedResponse<QuoteDTO>>>(url);
            return View(items);
        }

        [HttpGet]
        public async Task<IActionResult> Details(Guid id)
        {
            var response = await _apiService.GetAsync<ApiResponse<QuoteDTO>>($"api/Quote/{id}");
            var item = response?.Data;
            if (item == null) return NotFound();
            return View(item);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(Guid id)
        {
            var response = new string[] { };

            try
            {
                var success = await _apiService.DeleteAsync($"api/Quote/{id}");
                if (!success)
                {
                    response = new[] { "False", "Deletion failed" };
                    return Json(response);
                }

                response = new[] { "True", "Deleted successfully" };
                return Json(response);
            }
            catch (HttpRequestException ex)
            {
                response = new[] { "False", ex.Message };
                return Json(response);
            }
            catch (Exception)
            {
                response = new[] { "False", "Unexpected error" };
                return Json(response);
            }
        }
    }
}
