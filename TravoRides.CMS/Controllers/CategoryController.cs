using Microsoft.AspNetCore.Mvc;
using TravoRides.Application.Common.Responses;
using TravoRides.Application.DTOs.Common;
using TravoRides.Application.DTOs.Category;
using TravoRides.CMS.Interface;
using TravoRides.Domain.Entities;

namespace TravoRides.CMS.Controllers
{
    public class CategoryController : Controller
    {
        private readonly IApiService _apiService;

        public CategoryController(IApiService apiService)
        {
            _apiService = apiService;
        }
        [HttpGet]
        public async Task<IActionResult> Index(int? pageNumber, int? pageSize)
        {
            var page = pageNumber ?? 1;
            var size = pageSize ?? 10;

            var url = $"api/Category?pageNumber={page}&pageSize={size}";

            var items = await _apiService.GetAllAsync<ApiResponse<PagedResponse<CategoryDTO>>>(url);
            return View(items);
        }


        [HttpGet]
        public async Task<IActionResult> Details(Guid id)
        {
            var response = await _apiService.GetAsync<ApiResponse<CategoryDTO>>($"api/Category/{id}");
            var item = response?.Data;
            if (item == null) return NotFound();
            return View(item);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View(new CreateCategoryRequest());
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateCategoryRequest model)
        {
            if (!ModelState.IsValid)
            {
                return Json(new[] { "False", "Validation Failed" });
            }

            var apiResponse = await _apiService.PostAsync<
                CreateCategoryRequest, ApiResponse<object>>( "api/Category", model);

            if (apiResponse == null || !apiResponse.IsSuccess)
            {
                return Json(new[]
                {
            "False",
            apiResponse?.Message ?? "Category creation failed."
        });
            }

            return Json(new[]
            {
        "True",
        apiResponse.Message ?? "Category created successfully."
    });
        }
        [HttpGet]
        public async Task<IActionResult> Edit(Guid id)
        {
            var response = await _apiService.GetAsync<ApiResponse<CategoryDTO>>($"api/Category/{id}");
            var item = response?.Data;
            if (item == null) return NotFound();

            var model = new UpdateCategoryRequest
            {
                Id = item.Id,
               Name = item.Name,
               Description = item.Description
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Guid id, UpdateCategoryRequest model)
        {
            var response = new string[] { };

            if (!ModelState.IsValid)
            {
                response = new[] { "False", "Validation Failed" };
                return Json(response);
            }

            //var category = new UpdateCategoryRequest
            //{
            //    Id = id,
            //    Name = model.Name,
            //    Description = model.Description
            //};

            await _apiService.PutAsync<UpdateCategoryRequest,ApiResponse<object>>($"api/Category/{id}", model);

            response = new[] { "True", "Updated successfully." };
            return Json(response);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(Guid id)
        {
            var response = new string[] { };

            try
            {
                var success = await _apiService.DeleteAsync($"api/Category/{id}");
                if (!success)
                {
                    response = new[] { "False", "Deletion failed" };
                    return Json(response);
                }

                response = new[] { "True", "Deleted successfully" };
                return Json(response);
            }
            catch (Exception ex)
            {
                response = new[] { "False", ex.Message };
                return Json(response);
            }
        }
    }
}
