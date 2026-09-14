using TravoRides.Application.Common.Responses;
using TravoRides.Application.DTOs.Common;
using TravoRides.Application.DTOs.LatestThinking;
using TravoRides.Application.DTOs.Review;
using TravoRides.Application.Repositories;
using TravoRides.CMS.Interface;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Net.Http.Headers;

namespace TravoRides.CMS.Controllers
{
    public class ReviewController : Controller
    {
        private readonly IApiService _apiService;

        public ReviewController(IApiService apiService)
        {
            _apiService = apiService;
        }
        [HttpGet]
        public async Task<IActionResult> Index(int? pageNumber, int? pageSize)
        {
            var page = pageNumber ?? 1;
            var size = pageSize ?? 10;

            var url = $"api/Review/Admin?pageNumber={page}&pageSize={size}";

            var items = await _apiService.GetAllAsync<ApiResponse<PagedResponse<ReviewDTO>>>(url);
            return View(items);
        }
        [HttpGet]
        public async Task<IActionResult> Details(Guid id)
        {
            var response = await _apiService.GetAsync<ApiResponse<ReviewDTO>>($"api/Review/{id}");
            var item = response?.Data;
            if (item == null) return NotFound();
            return View(item);
        }

        [HttpPost]
        public async Task<IActionResult> ToggleReviewStatus(Guid id)
        {
            try
            {
                var response = await _apiService.GetAsync<ApiResponse<ReviewDTO>>($"api/Review/{id}");
                if (response?.Data == null)
                {
                    return Json(new { success = false, message = "Review not found" });
                }

                var dto = response.Data;
                dto.IsActive = !dto.IsActive;

                // send only status update to the API status endpoint
                var statusPayload = new { IsActive = dto.IsActive };
              //  await _apiService.PutAsync($"api/Review/{id}/status", statusPayload);

                return Json(new
                {
                    success = true,
                    status = dto.IsActive,
                    message = dto.IsActive ? "Review approved" : "Review unapproved"
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error occurred: " + ex.Message });
            }
        }
    }
}
