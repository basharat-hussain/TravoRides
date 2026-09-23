using Microsoft.AspNetCore.Mvc;
using TravoRides.Application.Common.Responses;
using TravoRides.Application.DTOs.SelfDrive;
using TravoRides.Application.DTOs.Category;
using TravoRides.Application.DTOs.Common;
using TravoRides.CMS.Interface;
using TravoRides.Application.DTOs.Cabs;

namespace TravoRides.CMS.Controllers
{
    public class SelfDriveController : Controller
    {
        private readonly IApiService _apiService;
        public SelfDriveController(IApiService apiService)
        {
            _apiService = apiService;
        }

        [HttpGet]
        public async Task<IActionResult> Index(int? pageNumber, int? pageSize)
        {
            var page = pageNumber ?? 1;
            var size = pageSize ?? 10;

            var url = $"api/SelfDrive?pageNumber={page}&pageSize={size}";

            var items = await _apiService.GetAllAsync<ApiResponse<PagedResponse<CabDTO>>>(url);
            return View(items);
        }

        [HttpGet]
        public async Task<IActionResult> Details(Guid id)
        {
            var response = await _apiService.GetAsync<ApiResponse<SelfDriveDTO>>($"api/SelfDrive/{id}");
            var item = response?.Data;
            if (item == null) return NotFound();
            return View(item);
        }
        [HttpGet]
        public async Task<IActionResult> GetCabsByCategory(Guid categoryId)
        {
            var response = await _apiService.GetAsync<ApiResponse<List<CabDTO>>>(
                $"api/Cab/by-category/{categoryId}");

            return Json(response);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var response = await _apiService.GetAsync<ApiResponse<PagedResponse<CategoryDTO>>>
                ("api/Category?pageNumber=1&pageSize=50");

            ViewBag.Categories = response.Data.Items ?? new List<CategoryDTO>();

            return View(new CreateSelfDriveRequest());
        }
        [HttpPost]
        public async Task<IActionResult> Create(CreateSelfDriveRequest model)
        {
            if (!ModelState.IsValid)
            {
                return Json(new[] { "False", "Validation Failed" });
            }

            var apiResponse = await _apiService.PostAsync<
                CreateSelfDriveRequest, ApiResponse<object>>("api/SelfDrive", model);

            if (apiResponse == null || !apiResponse.IsSuccess)
            {
                return Json(new[]
                {
            "False",
            apiResponse?.Message ?? "SelfDrive creation failed."
        });
            }

            return Json(new[]
            {
        "True",
        apiResponse.Message ?? "SelfDrive created successfully."
        });
        }
       
        [HttpGet]

        public async Task<IActionResult> Edit(Guid id)
        {
            var response =await _apiService.GetAsync<ApiResponse<SelfDriveDTO>>($"api/SelfDrive/{id}");
            var item = response?.Data;
            if (item == null) return NotFound();

            var categoryResponse = await _apiService.GetAsync<ApiResponse<PagedResponse<CategoryDTO>>>(
                    "api/Category?pageNumber=1&pageSize=100");
            ViewBag.Categories = categoryResponse.Data?.Items ?? new List<CategoryDTO>();

            var model = new UpdateSelfDriveRequest
            {
                Id = item.Id,
                PricePerDay = item.PricePerDay,
                Discount = item.Discount
            };

            return View(model);
        }
 
        [HttpPost]
        public async Task<IActionResult> Edit(Guid id, UpdateSelfDriveRequest model)
        {
            var response = new string[] { };

            if (!ModelState.IsValid)
            {
                response = new[] { "False", "Validation Failed" };
                return Json(response);
            }

            //var category = new UpdateSelfDriveRequest
            //{
            //    Id = id,
            //    PricePerDay = model.PricePerDay,
            //    Discount = model.Discount
            //};

            await _apiService.PutAsync<UpdateSelfDriveRequest, ApiResponse<object>>($"api/SelfDrive/{id}", model);

            response = new[] { "True", "Updated successfully." };
            return Json(response);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(Guid id)
        {
            var response = new string[] { };

            try
            {
                var success = await _apiService.DeleteAsync($"api/SelfDrive/{id}");
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
