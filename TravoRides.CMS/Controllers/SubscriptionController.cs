using AlArwaSolutions.Application.Common.Responses;
using AlArwaSolutions.Application.DTOs.Common;
using AlArwaSolutions.Application.DTOs.Subscription;
using AlArwaSolutions.CMS.Interface;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;

namespace AlArwaSolutions.CMS.Controllers
{
    public class SubscriptionController : Controller
    {
        private readonly IApiService _apiService;
        public SubscriptionController(IApiService apiService)
        {
            _apiService = apiService;
        }
        [HttpGet]
        public async Task<IActionResult> Index(int? pageNumber, int? pageSize)
        {
            var page = pageNumber ?? 1;
            var size = pageSize ?? 10;

            var url = $"api/Subscription?pageNumber={page}&pageSize={size}";

            var items = await _apiService.GetAllAsync<ApiResponse<PagedResponse<SubscriptionDTO>>>(url);
            return View(items);
        }
        [HttpGet]
        public async Task<IActionResult> Details(Guid id)
        {
            var response = await _apiService.GetAsync<ApiResponse<SubscriptionDTO>>($"api/Subscription/{id}");
            var item = response?.Data;
            if (item == null) return NotFound();
            return View(item);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View(new CreateSubscriptionRequest());
        }
        [HttpPost]
        public async Task<IActionResult> Create(CreateSubscriptionRequest model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            using var formData = new MultipartFormDataContent();
            formData.Add(new StringContent(model.Email ?? string.Empty), nameof(model.Email));

            await _apiService.PostAsync<ApiResponse<object>>("api/Subscription", formData);
            return RedirectToAction("Index");
        }

    }
}
