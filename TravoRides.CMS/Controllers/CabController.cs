using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using TravoRides.Application.Common.Responses;
using TravoRides.Application.DTOs.Cabs;
using TravoRides.Application.DTOs.Category;
using TravoRides.Application.DTOs.Common;
using TravoRides.Application.Interfaces;
using TravoRides.CMS.Interface;

namespace TravoRides.CMS.Controllers
{
    public class CabController : Controller
    {
        private readonly IApiService _apiService;
        public CabController(IApiService apiService)
        {
            _apiService = apiService;
        }

        [HttpGet]
        public async Task<IActionResult> Index(int? pageNumber, int? pageSize)
        {
            var page = pageNumber ?? 1;
            var size = pageSize ?? 10;

            var url = $"api/Cab?pageNumber={page}&pageSize={size}";

            var items = await _apiService.GetAllAsync<ApiResponse<PagedResponse<CabDTO>>>(url);
            return View(items);
        }

        [HttpGet]
        public async Task<IActionResult> Details(Guid id)
        {
            var response = await _apiService.GetAsync<ApiResponse<CabDTO>>($"api/Cab/{id}");
            var item = response?.Data;
            if (item == null) return NotFound();
            return View(item);
        }

        [HttpGet]

        public async Task<IActionResult> Create()
        {
            var response = await _apiService.GetAsync<ApiResponse<PagedResponse<CategoryDTO>>>
                ("api/Category?pageNumber=1&pageSize=20");

            ViewBag.Categories = response.Data.Items;

            return View(new CreateCabRequest());
        }
        [HttpPost]
        public async Task<IActionResult> Create(CreateCabRequest model)
        {
            var response = new string[] { };

            if (!ModelState.IsValid)
            {
                response = new[] { "False", "Validation Failed" };
                return Json(response);
            }

            using var formData = new MultipartFormDataContent();

            formData.Add(new StringContent(model.Name ?? string.Empty), nameof(model.Name));
            formData.Add(new StringContent(model.PricePerDay.ToString() ?? string.Empty), nameof(model.PricePerDay));
            formData.Add(new StringContent(model.Discount.ToString() ?? string.Empty), nameof(model.Discount));
            formData.Add(new StringContent(model.Transmission ?? string.Empty), nameof(model.Transmission));
            formData.Add(new StringContent(model.Description ?? string.Empty), nameof(model.Description));
            formData.Add(new StringContent(model.LuggageCapacity.ToString() ?? string.Empty), nameof(model.LuggageCapacity));
            formData.Add(new StringContent(model.SeatingCapacity.ToString() ?? string.Empty), nameof(model.SeatingCapacity));
            formData.Add(new StringContent(model.IsSelfDrive.ToString()), nameof(model.IsSelfDrive));

            // Category
            formData.Add( new StringContent(model.CategoryId.ToString()), nameof(model.CategoryId));

            // Fuel
            formData.Add( new StringContent(model.Fuel.ToString()), nameof(model.Fuel));

            if (model.Image != null && model.Image.Length > 0)
            {
                var imageContent = new StreamContent(model.Image.OpenReadStream());
                imageContent.Headers.ContentType = new MediaTypeHeaderValue(model.Image.ContentType);
                formData.Add(imageContent, nameof(model.Image), model.Image.FileName);
            }

            await _apiService.PostAsync<ApiResponse<object>>("api/Cab", formData);

            response = new[] { "True", "Created successfully." };
            return Json(response);
        }
        [HttpGet]
        public async Task<IActionResult> Edit(Guid id)
        {
            var response = await _apiService.GetAsync<ApiResponse<CabDTO>>($"api/Cab/{id}");
            var item = response?.Data;
            if (item == null) return NotFound();
            // Get categories for dropdown
            var categoryResponse = await _apiService.GetAsync<
            ApiResponse<PagedResponse<CategoryDTO>>>("api/Category?pageNumber=1&pageSize=100");

            ViewBag.Categories = categoryResponse.Data?.Items ?? new List<CategoryDTO>();
            var model = new UpdateCabRequest
            {
                Id = item.Id,
                CategoryId = item.Category.Id,
                Name = item.Name,
                PricePerDay = item.PricePerDay,
                Discount = item.Discount,
                Description = item.Description,
                Transmission = item.Transmission,
                Fuel = item.Fuel,
               LuggageCapacity = item.LuggageCapacity,
               SeatingCapacity = item.SeatingCapacity,
                ImageUrl = item.ImageUrl,
                IsSelfDrive = item.IsSelfDrive
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Guid id, UpdateCabRequest model)
        {
            var response = new string[] { };

            if (!ModelState.IsValid)
            {
                response = new[] { "False", "Validation Failed" };
                return Json(response);
            }

            using var formData = new MultipartFormDataContent();

            formData.Add(new StringContent(model.Name ?? string.Empty), nameof(model.Name));
            formData.Add(new StringContent(model.PricePerDay.ToString() ?? string.Empty), nameof(model.PricePerDay));
            formData.Add(new StringContent(model.Discount.ToString()?? string.Empty), nameof(model.Discount));
            formData.Add(new StringContent(model.Transmission ?? string.Empty), nameof(model.Transmission));
            formData.Add(new StringContent(model.Description ?? string.Empty), nameof(model.Description));
            formData.Add(new StringContent(model.CategoryId.ToString()), nameof(model.CategoryId));
            formData.Add(new StringContent(model.Fuel.ToString()), nameof(model.Fuel));
            formData.Add(new StringContent(model.LuggageCapacity.ToString() ?? string.Empty), nameof(model.LuggageCapacity));
            formData.Add(new StringContent(model.SeatingCapacity.ToString() ?? string.Empty), nameof(model.SeatingCapacity));
            formData.Add(new StringContent(model.IsSelfDrive.ToString()), nameof(model.IsSelfDrive));


            if (model.Image != null && model.Image.Length > 0)
            {
                var imageContent = new StreamContent(model.Image.OpenReadStream());
                imageContent.Headers.ContentType = new MediaTypeHeaderValue(model.Image.ContentType);
                formData.Add(imageContent, nameof(model.ImageUrl), model.Image.FileName);
            }

            await _apiService.PutAsync<ApiResponse<object>>($"api/Cab/{id}", formData);

            response = new[] { "True", "Updated successfully." };
            return Json(response);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(Guid id)
        {
            var response = new string[] { };

            try
            {
                var success = await _apiService.DeleteAsync($"api/Cab/{id}");
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
